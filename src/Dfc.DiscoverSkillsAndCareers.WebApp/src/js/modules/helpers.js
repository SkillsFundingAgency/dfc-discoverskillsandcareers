var helpers = (function () {
  return {
    isPage: function (className) {
      return document.getElementsByClassName(className).length
    }
  }
})()

module.exports = helpers
