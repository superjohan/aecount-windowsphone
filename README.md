Aecount for Windows Phone
=========================

This is the Windows Phone port of my iOS app called ["Aecount"](https://github.com/superjohan/aecount). It is a very simple gesture-based counter app. You can set the title and goal.

The app is still very much work-in-progress. I only started writing in order to learn both C# and the Windows Phone frameworks better. Feedback on code quality is very welcome, and encouraged. I'm sure the code is full of iOS-isms. (in fact, the SettingsHelper class is quite NSUserDefaults-influenced...)

Implemented so far:
- Increment. (tap on the count, or swipe to the left)
- Decrement. (swipe to the right)
- Increment/decrement animations.
- Reset the count to 0 by pinching.
- All values are saved to IsolatedStorageSettings.
- Pinch animations.
- Visual representation of the goal.

Things in the iOS version that are still missing in the Windows Phone version:
- Sounds.

Aecount uses the [Chunk](http://www.theleagueofmoveabletype.com/chunk) typeface by Meredith Mandel. I've included it directly in the repository. If this is a problem, please contact me.

Links
=====

[Aero Deko](http://aerodeko.com)

[Johan (@suprjohan) on Twitter](http://twitter.com/suprjohan)

[Johan (@jkh) on App.net](http://alpha.app.net/jkh)
