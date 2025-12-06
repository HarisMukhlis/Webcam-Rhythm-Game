---
<div align="center">
<h1>BEAT TOUCHER</h1>
<em>(working title)</em>
</div>

A webcam-based rhythm game prototype made in **Unity 6**. It uses your connected webcam device to read and tracks your body's gesture using Mediapipe's **Pose Estimation** model as an input, which as of now only grabs your left and right hands - _more specifically, wrists_. It runs on Python, then sent to the .NET framework using you local UDP Client, through **Socket**.

Inspired by games like Beat Saber, Cytus, and Osu!. Although I do realize that there's a similar game titled "Starri", I didn't know of this until I actually researched it mid-development.

This is a campus assignment made in a week. Since I rushed some late-implemented systems, expect some bugs and dirty techniques in certain areas lol.

### How To Use

Open the ```pose-estimation.py``` file first, install all the requirements by running 
```bash
pip install -r requirements.txt
```
(make sure to run the command within the same path as the requirements.txt file). Once finished, run the Python file a few times until it opens up a small webcam window with some landmarks detected (turn these on/off from the ```DEBUG_MODE = True``` variable). _If it cannot open after a few attempts, try changing the ```cap = cv2.VideoCapture(0)```'s number parameter, it depends on how many camera's are actually connected with your device._

Once it runs, open up the Unity Assets project (after you also install all the required packages. Use Unity 6+, with the renderer template of 2D URP). Then, run the "SelectionScreen" scene and try to move your hands while in focus.

### Features

A good base rhythm game concept in the Unity itself. It has:

* Solid and responsive UI layout.
* Fully fleshed out basic rhythm mechanics, complete with judgment scoring, note types, and combo mechanics.
* Easily manageable and (somewhat) robust charting system. It uses a scriptable object to save in Timeline/Playable asset with custom Signal Emitter, then converted into an array to read out later.
* Good enough visual and auditory feel.

### Tools/Libraries

For Python, I use these libraries:
* OpenCV
* Mediapipe
* Socket
* Numpy

For the Unity itself, I use these packages:
* DOTween
* TMP

### Notes

_I do wish to improve this project in the future. I would like to remove the delay/latency problem as best as I could, possibly by detaching Python completely out of the system, and run Mediapipe straight within Unity itself. Can't promise anything, though. If I did made this happen, I would definitely upload it on itch.io or steam._
