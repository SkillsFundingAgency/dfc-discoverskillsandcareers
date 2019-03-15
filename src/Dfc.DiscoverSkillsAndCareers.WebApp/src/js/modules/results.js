var results = (function () {
  return {
    init: function () {
      const resultsList = document.getElementById('app-results-list')
      const resultsItems = Array.prototype.slice.call(resultsList.children)

      const other = resultsItems.filter(result => resultsItems.indexOf(result) >= 3)

      if (other.length) {
        other.map(item => {
          item.style.display = 'none'
        })

        const wrapperElement = document.createElement('div')
        wrapperElement.classList.add('app-results-load-more')

        // "See x other job…" title
        const titleElement = document.createElement('h2')
        const titleText = 'See ' + other.length + ' other job categories you are suited to'
        titleElement.innerHTML = titleText

        wrapperElement.appendChild(titleElement)

        // "See matches" button
        const buttonElement = document.createElement('p')
        const buttonText = '<a class="govuk-link govuk-link--no-visited-state" href="">See matches</a>'
        buttonElement.innerHTML = buttonText

        wrapperElement.appendChild(buttonElement)

        // Append everything to container
        resultsList.parentNode.appendChild(wrapperElement)

        buttonElement.addEventListener('click', function (event) {
          event.preventDefault()
          other.map(item => {
            item.style.display = 'block'
          })
          titleElement.parentNode.removeChild(titleElement)
          buttonElement.parentNode.removeChild(buttonElement)
          return false
        })
      }
    }
  }
})()

module.exports = results
