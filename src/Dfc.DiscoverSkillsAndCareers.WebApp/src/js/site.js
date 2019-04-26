const helpers = require('./modules/helpers.js')
const analytics = require('./modules/analytics.js')
const results = require('./modules/results.js')
const breadcrumbs = require('./modules/breadcrumbs.js')
const navigation = require('./modules/navigation.js')

analytics.init()
breadcrumbs.init()
navigation.init()

if (helpers.isPage('app-page--results')) {
  results.short()
}

if (helpers.isPage('app-page--results-long')) {
  if (document.body.clientWidth >= 768) {
    results.cardHeight()
  }
  results.long()
}
