// browser.js
// 2018-06-16
// Copyright (c) 2015 Douglas Crockford  (www.JSLint.com)

/*jslint
    browser
*/

/*property
    checked, create, disable, display, error, focus, forEach, function,
    getElementById, innerHTML, join, length, map, onchange, onclick, onscroll,
    property, querySelectorAll, scrollTop, select, split, style, title, value
*/

import jslint from "./jslint.js";
import report from "./report.js";

// This is the web script companion file for JSLint. It includes code for
// interacting with the browser and displaying the reports.

let rx_separator = /[\s,;'"]+/;

function call_jslint(source) {

    //lint js file
    let result = jslint(
        source,
        option,
        (
            global_string === ""
            ? undefined
            : global_string.split(rx_separator)
        )
    );

    //generate html snippets
    let error_html = report.error(result);
    let function_html = report.function(result);
    let property_text = report.property(result);

    //get dom elements
    let warnings_list = document.getElementById("JSLINT_WARNINGS_LIST");
    let warnings = document.getElementById("JSLINT_WARNINGS");
    let report_list = document.getElementById("JSLINT_REPORT_LIST");
    let report_field = document.getElementById("JSLINT_REPORT");
    let property = document.getElementById("JSLINT_PROPERTY");
    let property_fieldset = document.getElementById("JSLINT_PROPERTYFIELDSET");

    //inject html in dom
    warnings_list.innerHTML = error_html;
    warnings.style.display = (
        error_html.length === 0
        ? "none"
        : "block"
    );

    report_list.innerHTML = function_html;
    report_field.style.display = "block";
    if (property_text) {
        property.value = property_text;
        property_fieldset.style.display = "block";
        property.scrollTop = 0;
    } else {
        property_fieldset.style.display = "none";
    }
}

//linting options
let option = {
    bitwise: true,
    convert: true,
    devel: true,
    eval: true,
    for: true,
    getset: true,
    long: true,
    single: true,
    this: true,
    white: true,
};

//allowed global variables
let global_string = "C3,SDK,this,extends,lang,super,constructor";
let source = `[@SOURCE@]`;

call_jslint(source);

