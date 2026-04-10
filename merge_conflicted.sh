#!/usr/bin/env bash
set -euo pipefail

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "Run this inside a git repository."
  exit 1
fi

CONFLICTS=()
while IFS= read -r line; do
  [ -n "$line" ] && CONFLICTS+=("$line")
done < <(git diff --name-only --diff-filter=U -- "*.png")

if [ "${#CONFLICTS[@]}" -eq 0 ]; then
  echo "No conflicted PNG files found."
  exit 0
fi

echo "Found ${#CONFLICTS[@]} conflicted PNG files."

for f in "${CONFLICTS[@]}"; do
  if ! git cat-file -e ":2:$f" 2>/dev/null || ! git cat-file -e ":3:$f" 2>/dev/null; then
    echo "Skipping $f (not a both-sides content conflict)."
    continue
  fi

  d="$(dirname "$f")"
  mkdir -p "$d"

  count=$(find "$d" -maxdepth 1 -type f -name "*.png" | wc -l | tr -d ' ')
  n1=$((count + 1))
  n2=$((count + 2))

  p1=$(printf "%s/%06d.png" "$d" "$n1")
  p2=$(printf "%s/%06d.png" "$d" "$n2")

  while [ -e "$p1" ] || [ -e "$p2" ]; do
    n1=$((n1 + 1))
    n2=$((n2 + 1))
    p1=$(printf "%s/%06d.png" "$d" "$n1")
    p2=$(printf "%s/%06d.png" "$d" "$n2")
  done

  git show ":2:$f" > "$p1"
  git show ":3:$f" > "$p2"

  git rm -f -- "$f" >/dev/null 2>&1 || true
  git add "$p1" "$p2"

  echo "$f -> $(basename "$p1"), $(basename "$p2")"
done

echo
echo "Done. Now run:"
echo "git add -A"
echo "git commit -m \"Merge drawings and renumber conflicted images\""