var navigation = (function () {
  return {
    init: function () {
      const navigation = document.getElementById('proposition-links')
      const toggle = document.getElementsByClassName('js-header-toggle')[0]
      if (navigation) {
        toggle.addEventListener('click', function (event) {
          toggle.style.display = 'none'
          event.preventDefault()
          navigation.style.display = 'block'
        })
      }
    }
  }
})()

module.exports = navigation
