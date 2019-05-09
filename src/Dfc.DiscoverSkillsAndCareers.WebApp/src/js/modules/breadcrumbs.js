var breadcrumbs = (function () {
  return {
    init: function () {
      const container = document.getElementsByClassName('app-browser-back')[0]
      if (container) {
        const linkText = document.createTextNode('Back')
        const link = document.createElement('a')
        link.className = 'govuk-back-link'
        link.setAttribute('href', '#')
        link.appendChild(linkText)
        link.addEventListener('click', function (event) {
          event.preventDefault()
          window.history.back()
        })
        container.appendChild(link)
      }
    }
  }
})()

module.exports = breadcrumbs
