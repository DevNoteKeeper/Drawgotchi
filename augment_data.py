import numpy as np
from PIL import Image, ImageFilter
import cv2
import pathlib


def center_ink(gray_img, threshold=245):
    mask = gray_img < threshold
    if not np.any(mask):
        return gray_img.copy()

    ys, xs = np.where(mask)
    y0, y1 = int(ys.min()), int(ys.max())
    x0, x1 = int(xs.min()), int(xs.max())

    crop = gray_img[y0 : y1 + 1, x0 : x1 + 1]
    h, w = gray_img.shape
    ch, cw = crop.shape
    out = np.full((h, w), 255, dtype=np.uint8)
    top = (h - ch) // 2
    left = (w - cw) // 2
    out[top : top + ch, left : left + cw] = crop
    return out


def resize_on_canvas(gray_img, scale):
    h, w = gray_img.shape
    new_w = max(1, int(round(w * scale)))
    new_h = max(1, int(round(h * scale)))
    resized = cv2.resize(gray_img, (new_w, new_h), interpolation=cv2.INTER_CUBIC)

    out = np.full((h, w), 255, dtype=np.uint8)

    src_x0 = max(0, (new_w - w) // 2)
    src_y0 = max(0, (new_h - h) // 2)
    dst_x0 = max(0, (w - new_w) // 2)
    dst_y0 = max(0, (h - new_h) // 2)

    copy_w = min(w, new_w)
    copy_h = min(h, new_h)
    out[dst_y0 : dst_y0 + copy_h, dst_x0 : dst_x0 + copy_w] = resized[
        src_y0 : src_y0 + copy_h, src_x0 : src_x0 + copy_w
    ]
    return out


def elastic_transform(gray_img):
    h, w = gray_img.shape
    alpha = np.random.uniform(8.0, 20.0)
    sigma = np.random.uniform(4.0, 8.0)

    dx = (np.random.rand(h, w).astype(np.float32) * 2.0 - 1.0)
    dy = (np.random.rand(h, w).astype(np.float32) * 2.0 - 1.0)
    dx = cv2.GaussianBlur(dx, (0, 0), sigmaX=sigma, sigmaY=sigma) * alpha
    dy = cv2.GaussianBlur(dy, (0, 0), sigmaX=sigma, sigmaY=sigma) * alpha

    grid_x, grid_y = np.meshgrid(np.arange(w, dtype=np.float32), np.arange(h, dtype=np.float32))
    map_x = grid_x + dx
    map_y = grid_y + dy

    return cv2.remap(
        gray_img,
        map_x,
        map_y,
        interpolation=cv2.INTER_LINEAR,
        borderMode=cv2.BORDER_CONSTANT,
        borderValue=255,
    )


def augment_and_save(input_dir, output_dir, multiplier=5):
    for class_dir in pathlib.Path(input_dir).iterdir():
        if not class_dir.is_dir():
            continue

        out_class = pathlib.Path(output_dir) / class_dir.name
        out_class.mkdir(parents=True, exist_ok=True)

        for img_path in class_dir.glob("*.png"):
            base = np.array(Image.open(img_path).convert("L"), dtype=np.uint8)
            base = center_ink(base)

            for i in range(multiplier):
                aug = base.copy()
                h, w = aug.shape

                # Random rotation around image center
                angle = np.random.uniform(-20, 20)
                mat = cv2.getRotationMatrix2D((w / 2.0, h / 2.0), angle, 1.0)
                aug = cv2.warpAffine(
                    aug,
                    mat,
                    (w, h),
                    flags=cv2.INTER_CUBIC,
                    borderMode=cv2.BORDER_CONSTANT,
                    borderValue=255,
                )

                # Random scale with centered fit/crop
                aug = resize_on_canvas(aug, np.random.uniform(0.85, 1.15))

                # Elastic distortion (better stroke shape variation)
                if np.random.random() > 0.25:
                    aug = elastic_transform(aug)

                # Slight blur simulates shaky lines
                if np.random.random() > 0.5:
                    aug = np.array(
                        Image.fromarray(aug).filter(
                            ImageFilter.GaussianBlur(radius=np.random.uniform(0, 1.2))
                        ),
                        dtype=np.uint8,
                    )

                # Add noise to simulate pencil texture
                noise = np.random.normal(0, 8, aug.shape)
                aug = np.clip(aug.astype(np.float32) + noise, 0, 255).astype(np.uint8)

                # Keep doodle centered after geometric transforms
                aug = center_ink(aug)

                Image.fromarray(aug).save(out_class / f"{img_path.stem}_aug{i}.png")

augment_and_save("dataset", "augmented_data")
