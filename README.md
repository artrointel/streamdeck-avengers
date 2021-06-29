# Avengers Key Plugin

#### Make your own key with animatable graphic effects. Multiple functions are also supported.



## Animatable Effects
You can add multiple animation effects **ordered by layer** in the effect property inspector.  
Make your own animation effects for your customized icon in stream deck !  

### Circle Spread
Spreads out with a circle. It can be used as touch feedback effect in your icon.  
![Sample-CircleSpread](./Images/sample_circle_spread.gif)
- Configurate color/alpha, delay/duration.

### Color Overlay
Makes Color overlay layer on your base image.  
![Sample-ColorOverlay](./Images/sample_color_overlay.gif)
- Configurate color/alpha, delay/duration.

### Flash
Flash the icon with color. It can be used as touch feedback effect in your icon.  
![Sample-Flash](./Images/sample_flash.gif)
- Configurate color/alpha, delay/duration.

### Pie
Draws Pie with color.  
![Sample-Pie](./Images/sample_pie.gif)
- Configurate color/alpha, delay/duration.
- Configurate grow/eat option and direction:Clockwise/Counter clockwise.

### Border Wave
Draws Moving waves on the border of the base image.  
![Sample-BorderWave](./Images/sample_border_wave.gif)  



## Effect Combination Examples
Each effect will be drawn **ordered by layer** in the effect property inspector.  

### Effect Combination
![Sample-Combination](./Images/example_combination.gif)  

### Skill icon like in game
Example of a Skill icon in game.  
![Sample-Skill](./Images/example_skill.gif)  
Cooltime as pie, Duration as border wave and color overlay, touch feedback as circle spread, Cooltime-end feedback as flash.  



## Functions
You can add multiple functions **ordered by layer** in the function property inspector.  
Make your own action for your customized icon in stream deck !  

### Open File/Folder
Opens File or folder.  
Put path of a file or a folder.  

### Open Webpage
Opens webpage like google.com  
Put address of a website.  

### Execute Command
Executes a command prompt.  
Put command like "shutdown -s -t 3600"  

### Key Combination
Dispatches Keystroke event. It can be used as a keyboard macro.  
- Record a key combination to be dispatched.
- Put duration and interval if you want to dispatch the keystroke recursively.

### Type Text
Text typing macro. It can be used as a keyboard macro.  
- Write a text to be dispatched.
- Put duration and interval if you want to dispatch the text recursively.


# Note
- Base Image Upload  
Do not upload image from stream deck menu manually.  
Use "Base Image : Update" button in the property inspector instead.  

# Support
**Issue/Bugs:** Post an issue in [GitHub](https://github.com/artrointel/streamdeck-avengers) with reproducible scenario if possible  
**Discussions/Suggestions:** DM to @artrointel in #developers-chat in Discord: [Bar Raiders](https://discord.gg/khpafQa) for quick reponse

# Change Log

### Version 1.0 is Out !
- Initial release for the Avengers Key.

# Platform
- Windows

# Future items
- Image: animated gif support as base image (in ver1.x)
- Image: lottie animation support as base image (in ver1.x)
- Effect: image blending animation effect (in ver1.x)
- Effect: image filtering animation effect (in ver1.x)
- Effect: text animation renderer (in ver1.x)
- Function: Play sound file (in ver1.x)
- Others: loop the key