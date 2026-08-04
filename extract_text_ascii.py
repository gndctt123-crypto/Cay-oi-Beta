import cv2
import numpy as np
import os

video_path = r"C:\Users\Admin\video.mp4"
output_dir = r"C:\Users\Admin\scratch_frames"
os.makedirs(output_dir, exist_ok=True)

cap = cv2.VideoCapture(video_path)

fps = cap.get(cv2.CAP_PROP_FPS)

frame_count = 0

while True:
    ret, frame = cap.read()
    if not ret:
        break
        
    frame_count += 1
    
    if frame_count % int(fps/3) == 0:
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
        
        lower_red1 = np.array([0, 100, 100])
        upper_red1 = np.array([10, 255, 255])
        mask_red1 = cv2.inRange(hsv, lower_red1, upper_red1)
        
        lower_red2 = np.array([160, 100, 100])
        upper_red2 = np.array([180, 255, 255])
        mask_red2 = cv2.inRange(hsv, lower_red2, upper_red2)
        
        mask_red = mask_red1 + mask_red2
        
        lower_black = np.array([0, 0, 0])
        upper_black = np.array([180, 255, 50])
        mask_black = cv2.inRange(hsv, lower_black, upper_black)
        
        mask_text = cv2.bitwise_or(mask_red, mask_black)
        
        kernel = np.ones((3,3), np.uint8)
        mask_text = cv2.morphologyEx(mask_text, cv2.MORPH_CLOSE, kernel)
        
        if cv2.countNonZero(mask_text) > 2000:
            b, g, r = cv2.split(frame)
            # Create alpha channel based on mask
            alpha = mask_text
            rgba = [b, g, r, alpha]
            dst = cv2.merge(rgba, 4)
            
            # Crop to the center where text usually is
            h, w = dst.shape[:2]
            center_dst = dst[int(h*0.2):int(h*0.8), int(w*0.1):int(w*0.9)]
            
            out_path = os.path.join(output_dir, f"text_frame_{frame_count}.png")
            cv2.imwrite(out_path, center_dst)
            print(f"Saved {out_path}")

cap.release()
