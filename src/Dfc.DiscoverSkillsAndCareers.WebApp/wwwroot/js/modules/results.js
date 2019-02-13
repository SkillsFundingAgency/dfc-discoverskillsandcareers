(function e(t, n, r) {
  function s(o, u) {
    if (!n[o]) {
      if (!t[o]) {
        var a = typeof require == "function" && require;
        if (!u && a) return a(o, !0);
        if (i) return i(o, !0);
        throw new Error("Cannot find module '" + o + "'");
      }

      var f = n[o] = {
        exports: {}
      };
      t[o][0].call(f.exports, function (e) {
        var n = t[o][1][e];
        return s(n ? n : e);
      }, f, f.exports, e, t, n, r);
    }

    return n[o].exports;
  }

  var i = typeof require == "function" && require;

  for (var o = 0; o < r.length; o++) s(r[o]);

  return s;
})({
  1: [function (require, module, exports) {
    var results = function () {
      return {
        init: function () {
          const resultsList = document.getElementById('app-results-list');
          const resultsItems = [...resultsList.children];
          const other = resultsItems.filter(result => resultsItems.indexOf(result) >= 3);

          if (other.length) {
            other.map(item => {
              item.style.display = 'none';
            });
            const wrapperElement = document.createElement('div');
            wrapperElement.classList.add('app-results-load-more'); // "See x other job…" title

            const titleElement = document.createElement('h2');
            const titleText = 'See ' + other.length + ' other job categories you are suited to';
            titleElement.innerHTML = titleText;
            wrapperElement.appendChild(titleElement); // "See matches" button

            const buttonElement = document.createElement('p');
            const buttonText = '<a class="govuk-link govuk-link--no-visited-state" href="">See matches</a>';
            buttonElement.innerHTML = buttonText;
            wrapperElement.appendChild(buttonElement); // Append everything to container

            resultsList.parentNode.appendChild(wrapperElement);
            buttonElement.addEventListener('click', function (event) {
              event.preventDefault();
              other.map(item => {
                item.style.display = 'block';
              });
              titleElement.remove();
              buttonElement.remove();
              return false;
            });
          }
        }
      };
    }();

    module.exports = results;
  }, {}]
}, {}, [1]);