
Joystick Profiles for VEGAS
--------------------------------

Since every joystick has different buttons, it is necessary to provide a
mapping between joystick buttons and what they do in the application.  This
is the purpose of the joystick mapping profiles.

The name of each joystick profile is based on the name returned by the joystick.
If there is not a profile for your joystick, a default profile is created, which
can then be edited to add more functionality.  Use this document and existing
profiles to map the buttons on your joystick to the functions you want.

The format of the joystick profiles is:
   [Section Name]
   Key=Values
Where "Section Name" is Focus1, Focus2, etc.  

Each focus is for a specific kind of control or window.  
Each focus has a set of numbered actions.
These actions are documented below.

To set a button on your joystick to do a certain action, use the form:
   Button=Action  Axis1 Scale1 Offset1  Axis2 Scale2 Offset2

For example,

   Button1=1  1 10000 0  2 -10000 0

Means that button 1 (usually the trigger) maps to action 1 (which, for the
surround panner, for example, is "set value") using the first and second 
joystick axis with no scaling and no offset.

The axis are:
1: X
2: Y
3: Z (rare)
4: X rotation (rare)
5: Y rotation (rare)
6: Z rotation (usually rudder, not used in any of the default profiles)
7: slider 1 (usually throttle, used in all of the default profiles)
8: slider 2

For joysticks that move in a circle instead of a square, use scaling
factors of 15000 (i.e. 1.5x) for the square controls (such as the surround
panner) to make it so you can move within the square.

In the above example, the second axis is "flipped" by using a scale of -10000.

The current set of focus areas and their actions are:

Focus1: Trackbar control
------------------------
1: Set value, X or Y axis (depending on orientation)
2: Set value, attenuation only (min to default), X or Y axis (depending on orientation)
3: Set to default value

Focus2: Fader control
---------------------
1: Set value, X or Y axis (depending on orientation)
2: Set value, attenuation only (min to default), X or Y axis (depending on orientation)
3: Set to default value

Focus3: Surround Panner control
-------------------------------
1: Set value, using X and Y axis
2: Center

Focus4: Placement control (reserved / not implemented)
-------------------------
1: Set value, using X and Y axis
2: Center

Focus5: Color Wheel control (used in Color Corrector)
---------------------------
1: Set value, using X and Y axis
2: Center

Focus6: Color Corrector Plugin
------------------------------
1: Previous wheel
2: Next wheel

Focus7: Vegas Surround Popup
----------------------------
Unassigned buttons are passed to the surround control, so make sure to leave the button assigned to Surround Panner control action 1 unassigned here
1: Previous track or bus
2: Next track or bus
3: Set center level (make sure the same button maps to action 1 of the trackbar control)
4: Set keyframe smoothness (make sure the same button maps to action 1 of the trackbar control)
5: Close window (useful if this is mapped to the same button as Vegas Main Track View action 2 and Vegas Mixer View action 2)
6: Change pan type
7: Change X/Y/Free bounding mode
8: Toggle LFE mode
9: Previous keyframe on track
10: Next keyframe on track

Focus8: Vegas Main Track View
-----------------------------
1: Set pan (pan trackbar or surround pan control, map to the same button as Trackbar action 1 and Surround Panner action 1)
2: Reset or Open pan (reset pan to center for stereo mode, open surround panner for surround mode; map to same button as Trackbar action 2 and Vegas Surround Popup action 5)
3: Previous track / bus
4: Next track / bus
5: Set assignable fader (must map to a button used with Trackbar action 1 or 2)
6: Set volume (must map to a button used with Fader action 1 or 2)
7: Previous keyframe on track
8: Next keyframe on track

Focus9: Vegas Mixer View
------------------------
1: Set surround pan (map to the same button as Surround Panner action 1)
2: Open pan (open surround panner; map to same button as Vegas Surround Popup action 5)
3: Previous bus
4: Next bus
5: Set volume (must map to a button used with Fader action 1 or 2)
6: Set input volume (for assignable FX; must map to a button used with Fader action 1 or 2)
