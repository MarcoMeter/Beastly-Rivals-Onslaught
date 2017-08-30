This is just an excerpt from the Powerbot Asset!

_______________________
	- Powerbot -
-----------------------

Created by Sergio Santos,
email: sergiossn@gmail.com
http://sergiosantosart.wordpress.com
http://www.knittedpixels.com


_______________________________________________________________________________________________________________________

Thank you for purchasing Powerbot.
Powerbot contains 14 variations (combining geometry and material variations) of the same character
and 30 animations using the same rig.

To use Powerbot in your project go the prefab folder and drag one of the prefabs in your scene.

In order to make it move you will need to add an script to tell to the animator which animation to play.

All the animations are already split in clips (see animacion clip section).
for more information on how to use mecanim follow this tutorial:
http://youtu.be/Xx21y9eJq1U

I hope you like this asset!


_______________________________________________________________________________________________________________________


FOLDERS:

- Models

  Contains the different models and animations used to create the prefabs for the characters.
  It also contains the Material Folder

    - Materials

	   It contains the materials used to create the variations of the character.


- Prefab Characters

   Contains the prefabs to be used in the game.
   They are already prepared with the right materials and the animator component.
   There are 14 variations to be used as different skins for the main character or as enemies as well.



- Scenes

   The scene with all the prefabs line up.


- Shaders

   The shader for the body of the character.
   It's a custom build shader to achieve a metallic material with reflection and rim light.


- Textures

   There are 5 textures and a cubemap.

   Blue with difuse and specular,
   Red with difuse and specular,
   white with difuse and specular (used to be multiplied so it can generate other colors easily)
   Normal map to achive the extra details of the bump,
   and reflection to be used in the reflection slot in the material, it's also be used to create the cubemap.

_______________________________________________________________________________________________________________________

ANIMATION

All the animation clips are splited in the Animations file inside the Models folder.
The model change position in the animation, to play the animation in the same spot (not moving)
select the clip you want to change and "Bake Into Pose" should be clicked.

Also in the Prefab you should have "Apply Root Motion" unchecked if you want to move your character by code
instead of using the motion in the animation.


Animations Clips:

  10 -  49  idle
  55 -  95  jump
  55 -  70  jump up
  75 -  78  falling
  79 -  94  touch ground
  96 - 120  run fwd
 126 - 155  die-fall back
 174 - 198  run w/ gun
 207 - 215  free fall
 222 - 232  shot stand
 235 - 250  get hit
 250 - 280  protection
 260 - 270  protect loop
 282 - 314  chest charge
 320 - 360  jump w/ gun
 364 - 385  floor slide
 391 - 415  super punch
 421 - 460  super uppercut
 465 - 506  crouch idle
 509 - 533  climb ladder
 600 - 621  climb ceiling start
 612 - 655  climb ceiling loop
 655 - 683  climb ceiling end
 689 - 736  charge energy begining/end
 710 - 724  charging energy loop
 742 - 748  jump strike down start
 749 - 751  jump strike down loop
 751 - 767  jump strike down finish
 769 - 797  strike down

 Animations@Walk

 800 - 848  Walk

