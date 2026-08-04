import cv2
import numpy as np
import os

video_path = r"C:\Users\Admin\Videos\Bản ghi màn hình\Quay màn hình 2026-07-29 101040.mp4"
output_dir = r"C:\Users\Admin\Cay ơi\scratch"
os.makedirs(output_dir, exist_ok=True)

cap = cv2.VideoCapture(video_path)
if not cap.isOpened():
    print("Error opening video file")
    exit(1)

fps = cap.get(cv2.CAP_PROP_FPS)
print(f"Video FPS: {fps}")

frame_count = 0
saved_count = 0

while True:
    ret, frame = cap.read()
    if not ret:
        break
        
    frame_count += 1
    
    # Process only 2 frames per second
    if frame_count % int(fps/2) == 0:
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
        
        # Red can be at both ends of the H spectrum (0-10 and 160-180)
        lower_red1 = np.array([0, 100, 100])
        upper_red1 = np.array([10, 255, 255])
        mask_red1 = cv2.inRange(hsv, lower_red1, upper_red1)
        
        lower_red2 = np.array([160, 100, 100])
        upper_red2 = np.array([180, 255, 255])
        mask_red2 = cv2.inRange(hsv, lower_red2, upper_red2)
        
        mask_red = mask_red1 + mask_red2
        
        # Also need black outlines. Black is V < 50
        lower_black = np.array([0, 0, 0])
        upper_black = np.array([180, 255, 50])
        mask_black = cv2.inRange(hsv, lower_black, upper_black)
        
        # Combine masks
        mask_text = cv2.bitwise_or(mask_red, mask_black)
        
        # Apply morphological operations to clean up
        kernel = np.ones((3,3), np.uint8)
        mask_text = cv2.morphologyEx(mask_text, cv2.MORPH_CLOSE, kernel)
        
        # Check if there is a significant amount of text
        if cv2.countNonZero(mask_text) > 5000:
            # Create transparent image
            b, g, r = cv2.split(frame)
            rgba = [b, g, r, mask_text]
            dst = cv2.merge(rgba, 4)
            
            # Crop to the center where text usually is
            h, w = dst.shape[:2]
            center_dst = dst[int(h*0.2):int(h*0.8), int(w*0.1):int(w*0.9)]
            
            cv2.imwrite(os.path.join(output_dir, f"text_frame_{frame_count}.png"), center_dst)
            saved_count += 1
            print(f"Saved frame {frame_count}")

cap.release()
print(f"Done. Saved {saved_count} frames.")
