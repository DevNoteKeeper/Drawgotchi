import tensorflow as tf
import numpy as np
from PIL import Image, ImageFilter
import os, pathlib


def augment_and_save(input_dir, output_dir, multiplier=10):
    for class_dir in pathlib.Path(input_dir).iterdir():
        out_class = pathlib.Path(output_dir) / class_dir.name
        out_class.mkdir(parents=True, exist_ok=True)

        for img_path in class_dir.glob("*.png"):
            img = Image.open(img_path).convert("L")

            for i in range(multiplier):
                aug = img

                # Random rotation (-20 to 20 deg)
                aug = aug.rotate(np.random.uniform(-20, 20), fillcolor=255)

                # Random scale
                w, h = aug.size
                scale = np.random.uniform(0.85, 1.15)
                aug = aug.resize((int(w * scale), int(h * scale)), Image.LANCZOS)
                aug = aug.crop((0, 0, w, h)) if scale > 1 else \
                    Image.new("L", (w, h), 255).paste(aug, (0, 0)) or aug

                # Slight blur (simulates shaky lines)
                if np.random.random() > 0.5:
                    aug = aug.filter(ImageFilter.GaussianBlur(radius=np.random.uniform(0, 1.2)))

                # Add noise (simulates pencil texture)
                noise = np.random.normal(0, 8, np.array(aug).shape)
                aug = Image.fromarray(np.clip(np.array(aug) + noise, 0, 255).astype(np.uint8))

                aug.save(out_class / f"{img_path.stem}_aug{i}.png")

augment_and_save("dataset", "augmented_data")