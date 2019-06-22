mergeInto(LibraryManager.library, {
  gameLoadedCallback: function () {
    console.log("WebGL: Robot Controller Loaded!");
	gameLoadedHandler();
  }
});