var results = (function () {
  function breakArrayIntoGroups (data, maxPerGroup) {
    var groups = []
    for (var index = 0; index < data.length; index += maxPerGroup) {
      groups.push(data.slice(index, index + maxPerGroup))
    }
    return groups
  }
  return {
    cardHeight: function () {
      const cards = Array.prototype.slice.call(document.getElementsByClassName('app-long-results__item'))
      var groups = breakArrayIntoGroups(cards, 3)
      groups.map(group => {
        var height = 0
        group.map(card => {
          var description = card.getElementsByClassName('result-description')[0]
          height = description.offsetHeight > height ? description.offsetHeight : height
        })
        group.map(card => {
          var description = card.getElementsByClassName('result-description')[0]
          description.style.height = height + 'px'
        })
      })
    },
    short: function () {
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
    },
    long: function () {
      const resultsLists = Array.prototype.slice.call(document.getElementsByClassName('app-long-results'))
      resultsLists.map(resultsList => {
        const resultsItems = Array.prototype.slice.call(resultsList.children)

        const other = resultsItems.filter(result => resultsItems.indexOf(result) >= 3)

        if (other.length) {
          other.map(item => {
            item.style.display = 'none'
          })

          const wrapperElement = resultsList.nextElementSibling.children[0].children[0]

          // "See matches" button
          const buttonElement = document.createElement('p')
          const buttonText = '<a class="govuk-link govuk-link--no-visited-state" href="">View ' + other.length + ' more results</a>'
          buttonElement.innerHTML = buttonText

          wrapperElement.appendChild(buttonElement)

          buttonElement.addEventListener('click', function (event) {
            event.preventDefault()
            other.map(item => {
              item.style.display = 'block'
            })
            buttonElement.parentNode.removeChild(buttonElement)
            return false
          })
        }
      })
    }
  }
})()

module.exports = results
