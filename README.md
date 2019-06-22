# BumbleBeer
A website that can control a physical beerpong robot that has 2 axis rotation, cup sensors and a coil gun.</br></br>
Watch it in operation on [YouTube](https://www.youtube.com/watch?v=oYCZETZMVRY "YouTube").</br></br>
![Banner.jpg](https://github.com/Fluctuationqt/BumbleBeer/blob/master/Banner.jpg "BumbleBeer")

## Contains
1. [Firmware](https://github.com/Fluctuationqt/BumbleBeer/blob/master/Firmware/StamatNodeMCU2/StamatNodeMCU2.ino)- The NodeMCU microcontroller code for the *Arduino IDE*.
2. [WebGLApp](https://github.com/Fluctuationqt/BumbleBeer/tree/master/WebGL%20App) - The right hand side of the control website. The full *Unity version 2017.1.1* Project.
3. [Control Website](https://github.com/Fluctuationqt/BumbleBeer/tree/master/Control%20Website/htdocs) - The Website for controling the robot. *PHP/JS/CSS/HTML*
4. [Schematics](https://github.com/Fluctuationqt/BumbleBeer/tree/master/Schematics) - Some of the schematics. *Will upload more on request!*

## Part List
* Microcontroller: NodeMCU</br>
* Servo: 10kg/cm (the pitch servo has been modified with an additional 2:1 gear reduction and an outboard potentiometer on the pitch axis because the mechanism can carry a 1kg DSLR with a telephoto lens if the barrel is removed.)</br>
 * The gun: A bank of capacitors at 660V that discharge into a coil that is arround the barrel. Inside the coil there is an elongated metal piton with a rebound spring that gets pushed when the caps discharge into the coil. This in term shoots the ping pong ball from the barrel at aprox 3m max distance and 10cm min distance depending on how much you charge the caps. The cap charging circuit (the white box) has a 2 relays that charge the caps from 220V AC through a diode and discharge them into the coil. The relays are connected to the NodeMCU via optocouplers for galvanic separation from the AC line. </br>
* The cup sensors are 3 pairs of an IR LED and an IR phototransistor that detect the reflection off of the ping pong balls when they go inside the cup.</br>
* The software: Apache/MySQL for the control website. It has a Unity WebGL app that sends out HTTP requests to the NodeMCU and visualizes the robot in 3D.  For the streaming i used a free software called YawCam it has an MJPEG streaming option and works flawlessly with low delay. The database holds a record of the robot's IP address in the Internal Network that gets set by the NodeMCU on startup.</br>

## PS
*Contact [me](mailto:outrageousxqt@gmail.com "My Email") for more information. I'll be glad to help out.*
