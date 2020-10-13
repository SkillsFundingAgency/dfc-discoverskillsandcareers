var results = (function () {
  const cookieName = '.dysac-result'
  const cookieData = getCookie(cookieName)
  const data = cookieData ? JSON.parse(cookieData) : null
  function breakArrayIntoGroups (data, maxPerGroup) {
    var groups = []
    for (var index = 0; index < data.length; index += maxPerGroup) {
      groups.push(data.slice(index, index + maxPerGroup))
    }
    return groups
  }
  function setCookie (name, value, days) {
    var expires = ''
    if (days) {
      var date = new Date()
      date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000))
      expires = '; expires=' + date.toUTCString()
    }
    document.cookie = name + '=' + (value || '') + expires + '; path=/'
  }
  function getCookie (name) {
    var value = '; ' + document.cookie
    var parts = value.split('; ' + name + '=')
    if (parts.length === 2) return parts.pop().split(';').shift()
  }
  function findAncestor (el, sel) {
    while ((el = el.parentElement) && !((el.matches || el.matchesSelector).call(el, sel)));
    return el
  }
  return {
    cardHeight: function () {
      const lists = Array.prototype.slice.call(document.getElementsByClassName('app-long-results'))
      lists.map(list => {
        const cards = Array.prototype.slice.call(list.getElementsByClassName('app-long-results__item'))
        var groups = breakArrayIntoGroups(cards, 3)
        groups.map(group => {
          var descriptionHeight = 0
          var salaryHeight = 0
          group.map(card => {
            var description = card.getElementsByClassName('result-description')[0]
            var salary = card.getElementsByClassName('result-detail--salary')[0]
            description.style.height = 'auto'
            salary.style.height = 'auto'
            descriptionHeight = description.offsetHeight > descriptionHeight ? description.offsetHeight : descriptionHeight
            salaryHeight = salary.offsetHeight > salaryHeight ? salary.offsetHeight : salaryHeight
          })
          group.map(card => {
            var description = card.getElementsByClassName('result-description')[0]
            var salary = card.getElementsByClassName('result-detail--salary')[0]
            description.style.height = descriptionHeight + 'px'
            salary.style.height = salaryHeight + 'px'
          })
        })
      })
    },
    short: function () {
      const resultsList = document.getElementById('app-results-list')
      const resultsItems = Array.prototype.slice.call(resultsList.children)
      const other = resultsItems.filter(result => resultsItems.indexOf(result) >= 3)

      var saveState = (state) => {
        let cookieData = getCookie(cookieName)
        let data = cookieData ? JSON.parse(cookieData) : {}
        data['general'] = state
        setCookie(cookieName, JSON.stringify(data))
      }

      function showItems (items, titleElement, buttonElement) {
        items.map(item => {
          item.style.display = 'block'
        })
        titleElement.parentNode.removeChild(titleElement)
        buttonElement.parentNode.removeChild(buttonElement)
      }

      if (other.length) {
        other.map(item => {
          item.style.display = 'none'
        })

        const wrapperElement = document.createElement('div')
        wrapperElement.classList.add('app-results-load-more')

        // "See x other job…" title
        const titleElement = document.createElement('h2')
        const titleText = other.length === 1 ? 'See ' + other.length + ' other career area you are suited to' : 'See ' + other.length + ' other career areas you are suited to'
        titleElement.innerHTML = titleText

        wrapperElement.appendChild(titleElement)

        // "See matches" button
        const buttonElement = document.createElement('p')
        const buttonText = '<a class="govuk-link govuk-link--no-visited-state" href="">See matches</a>'
        buttonElement.innerHTML = buttonText

        wrapperElement.appendChild(buttonElement)

        // Append everything to container
        resultsList.parentNode.appendChild(wrapperElement)

        if (data && data['general']) {
          showItems(other, titleElement, buttonElement)
        }

        buttonElement.addEventListener('click', function (event) {
          event.preventDefault()
          showItems(other, titleElement, buttonElement)
          saveState(true)
          return false
        })
      }
    },
    long: function () {
      const resultsLists = Array.prototype.slice.call(document.getElementsByClassName('app-long-results'))

      resultsLists.map(resultsList => {
        const showButtonElement = document.createElement('a')
        const hideButtonElement = document.createElement('a')
        const resultsItems = Array.prototype.slice.call(resultsList.children)
        const rowLength = 3
        const showMoreText = 'Show _count more profile'
        const showLessText = 'Show fewer profiles'
        const cards = resultsItems.filter(result => {
          return resultsItems.indexOf(result) >= 3
        })
        const numOfCards = cards.length

        // Split cards into groups of three and use cookie to see if we should be showing any
        const code = findAncestor(resultsList, '.app-results__item').dataset.jobFamilyCode
        const groups = breakArrayIntoGroups(cards, rowLength)
        const groupsToShow = data && code ? data[code] : null

        let groupIndex = groupsToShow || 0

        var updateCardHeight = () => {
          if (document.body.clientWidth >= 768) {
            this.cardHeight()
          }
        }

        var updateButtons = () => {
          if (getRemainingCards() > 0) {
            var amount = Math.min(3, getRemainingCards())
            showButtonElement.style.display = 'initial'
            showButtonElement.innerText = showMoreText.replace(/_count/g, amount)
            if (amount > 1) {
              showButtonElement.innerText = showButtonElement.innerText + 's'
            }
          } else {
            showButtonElement.style.display = 'none'
            showButtonElement.innerText = showMoreText
          }
          if (groupIndex > 0) {
            hideButtonElement.style.display = 'inline'
          } else {
            hideButtonElement.style.display = 'none'
          }
        }

        var saveState = () => {
          let cookieData = getCookie(cookieName)
          let data = cookieData ? JSON.parse(cookieData) : {}
          let code = findAncestor(resultsList, '.app-results__item').dataset.jobFamilyCode
          data[code] = groupIndex
          setCookie(cookieName, JSON.stringify(data))
        }

        var getRemainingCards = () => {
          return numOfCards - (groupIndex * rowLength)
        }

        if (groups.length) {
          groups.map((group, index) => {
            if (!groupsToShow || (groupsToShow && index >= groupsToShow)) {
              group.map(el => {
                el.style.display = 'none'
              })
            }
          })

          const wrapperElement = resultsList.nextElementSibling.children[0].children[0]

          // More button
          showButtonElement.classList.add('govuk-link', 'govuk-link--no-visited-state')
          showButtonElement.href = '#'
          showButtonElement.innerText = showMoreText
          wrapperElement.appendChild(showButtonElement)

          showButtonElement.addEventListener('click', function (event) {
            event.preventDefault()
            groups[groupIndex].map(el => {
              el.style.display = 'block'
            })
            groupIndex += 1
            updateButtons()
            saveState()
            updateCardHeight()
            return false
          })

          // Less button
          hideButtonElement.classList.add('govuk-link', 'govuk-link--no-visited-state')
          hideButtonElement.href = '#'
          hideButtonElement.innerText = showLessText
          wrapperElement.appendChild(hideButtonElement)

          hideButtonElement.addEventListener('click', function (event) {
            event.preventDefault()
            groups[groupIndex - 1].map(el => {
              el.style.display = 'none'
            })
            groupIndex -= 1
            updateButtons()
            saveState()
            updateCardHeight()
            return false
          })
        }

        updateButtons()
      })
    }
  }
})()

module.exports = results
