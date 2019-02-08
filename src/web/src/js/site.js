const isPage = className => document.getElementsByClassName(className).length

if (isPage('app-page--results')) {
  const resultsList = document.getElementById('app-results-list')
  const resultsItems = [...resultsList.children]

  const other = resultsItems.filter(result => resultsItems.indexOf(result) >= 3)

  if (other.length) {
    other.map(item => {
      item.className = 'visually-hidden'
    })

    const titleElement = document.createElement('h2')
    const titleText = 'See ' + other.length + ' other job categories you are suited to'
    titleElement.innerHTML = titleText

    resultsList.parentNode.appendChild(titleElement)

    const buttonElement = document.createElement('p')
    const buttonText = '<a class="govuk-link govuk-link--no-visited-state" href="">See matches</a>'
    buttonElement.innerHTML = buttonText

    resultsList.parentNode.appendChild(buttonElement)

    buttonElement.addEventListener('click', function (event) {
      event.preventDefault()
      other.map(item => {
        item.className = ''
      })
      titleElement.remove()
      buttonElement.remove()
      return false
    })
  }
}
