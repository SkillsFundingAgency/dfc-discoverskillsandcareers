const helpers = require('./modules/helpers.js')
const results = require('./modules/results.js')
const analytics = require('./modules/analytics.js')
const breadcrumbs = require('./modules/breadcrumbs.js')
const cookieprefrences = require('./modules/cookieprefrences.js')
const GOVUKFrontend = require('govuk-frontend')


GOVUKFrontend.initAll()
analytics.init()
breadcrumbs.init()

if (helpers.isPage('app-page--results')) {
  results.short()
}

if (helpers.isPage('app-page--results-long')) {
  if (document.body.clientWidth >= 768) {
    results.cardHeight()
  }
  results.long()
}


$(document).ready(function () {

    if (typeof cookieprefrences.window.GOVUK === 'undefined') document.body.className = document.body.className.replace('js-enabled', '');

    //only run this is on non setting pages
    if ($("#form-cookie-settings").length === 0) {
        if (cookieprefrences.window.GOVUK.cookie("cookies_preferences_set")) {
            $("#global-cookie-banner").hide();
        }
        else {
            //set defaults
            cookieprefrences.window.GOVUK.setConsentCookie();
            //give the browser time to set the cookies before acting on them
            setTimeout(function () { cookieprefrences.window.GOVUK.deleteUnconsentedCookies(); }, 500);
            setTimeout(function () { cookieprefrences.window.GOVUK.setGATracking(); }, 1000);
        }

        $("#accept-all-cookies").click(function () {

            cookieprefrences.window.GOVUK.approveAllCookieTypes();
            $("#cookie-message").hide();
            $("#confirmatiom-message").show();
            cookieprefrences.window.GOVUK.cookie('cookies_preferences_set', 'true', { days: 365 })
            setTimeout(function () { cookieprefrences.window.GOVUK.setGATracking(); }, 1000);
        });

        $("#hide-cookies-message").click(function () {
            $("#global-cookie-banner").hide();
        });
    }
});