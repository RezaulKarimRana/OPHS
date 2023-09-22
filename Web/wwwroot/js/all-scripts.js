
function setLanguageCookie(cookieValue) {
    var today = new Date();
    var expire = new Date();
    var cookieName = 'lang';
    //var cookieValue = "bn";
    var nDays = 5;
    expire.setTime(today.getTime() + 3600000 * 24 * nDays);
    document.cookie = cookieName + "=" + escape(cookieValue)
        + ";expires=" + expire.toGMTString();
}

function setLanguage() {
    $("#lang_form").submit();
    return false;
}

$(function () {

    // Slideshow 4
    $("#front-image-slider").responsiveSlides({
        auto: true,
        pager: false,
        nav: true,
        speed: 2000,
        maxwidth: 960,
        namespace: "callbacks"
    });
    $("#right-content a").click(function () {
        var url = $(this).attr('href');
        if (isExternal(url) && url != 'javascript:;') {
            openInNewTab(url);
            return false;
        }
    });
});
function openInNewTab(url) {
    var win = window.open(url, '_blank');
    win.focus();
}
function isExternal(url) {
    var match = url.match(/^([^:\/?#]+:)?(?:\/\/([^\/?#]*))?([^?#]+)?(\?[^#]*)?(#.*)?/);
    if (typeof match[1] === "string" && match[1].length > 0 && match[1].toLowerCase() !== location.protocol)
        return true;
    if (typeof match[2] === "string" && match[2].length > 0 && match[2].replace(new RegExp(":(" + {
        "http:": 80,
        "https:": 443
    }[location.protocol] + ")?$"), "") !== location.host)
        return true;
    return false;
}





$(function () {


    function initNewsTicker(id, options) {

        var $scroller = $(id);
        $scroller.vTicker('init', options);

        $("#news-ticker .next").click(function (event) {
            event.preventDefault();
            var checked = true;
            $('#news-ticker').vTicker('next', {animate: checked});
        });
        $("#news-ticker .prev,#news-ticker .next").hover(function () {
            $('#news-ticker').vTicker('next', {animate: checked});
            $scroller.vTicker('pause', true);
        }, function () {
            $('#news-ticker').vTicker('next', {animate: checked});
            $scroller.vTicker('pause', false);
        });
        $("#news-ticker .prev").click(function (event) {
            event.preventDefault();
            var checked = true;
            $('#news-ticker').vTicker('prev', {animate: checked});
        });
    }

    function initNoticeBoardTicker(id, options) {

        var $scroller = $(id);
        $scroller.vTicker('init', options);

        $("#notice-board-ticker .next").click(function (event) {
            event.preventDefault();
            var checked = true;
            $('#notice-board-ticker').vTicker('next', {animate: false});
        });
        $("#notice-board-ticker .prev,#notice-board-ticker .next").hover(function () {
            $scroller.vTicker('pause', true);
        }, function () {
            $scroller.vTicker('pause', false);
        });
        $("#notice-board-ticker .prev").click(function (event) {
            event.preventDefault();
            var checked = true;
            $('#notice-board-ticker').vTicker('prev', {animate: checked});
        });
    }

    initNewsTicker('#news-ticker', {});
    //initNoticeBoardTicker('#notice-board-ticker', {showItems:10, margin: 0, padding: 0,animate:false});

});


/* Responsive Menu*/








$(".meganizr .mzr-drop").keyup(function () {

    $(".mzr-content").attr("aria-hidden", "true");
    $(this).find(".mzr-content").attr("aria-hidden", "false");
});
// ============ start tile for <a> and alt for img ========
$('a').each(function () {
    $(this).attr('title', $(this).text());
});

var lan = "bn";
if (lan == 'en') {
    $('.mzr-drop a:first-child').each(function () {
        $(this).attr('title', "Enter to get more menu");
    });
} else {
    $('.mzr-drop a:first-child').each(function () {
        $(this).attr('title', "Menu");
    });
}
$('img').each(function () {
    var str = $(this).attr("src");
    var arr = str.split('/');
    var strFine = arr[arr.length - 1];

    var str2 = strFine;
    var arr2 = str2.split('.');
    var strFine2 = arr2[arr2.length - 2];
    $(this).attr('alt', strFine2);
});
$('a2iLogo').attr('alt', 'Access to information');
$('.service-box img').each(function () {
    var strTitle = $(this).parent().find('h4').text();
    $(this).attr('alt', strTitle);
});
$('.block img').each(function () {
    var strTitleRight = $(this).closest('.block').find('.title').text();
    $(this).attr('alt', strTitleRight);
});
$('#notice-board-ticker .btn').attr('title', 'à¦¸à¦•à¦² à¦¨à§‹à¦Ÿà¦¿à¦¶');
$('#news-ticker .btn').attr('title', 'à¦¸à¦•à¦² à¦–à¦¬à¦°');
$('#search').each(function () {
    $(this).attr('alt', 'Search');
});
$('.search-btn').each(function () {
    $(this).attr('alt', 'Enter to search');
});
$(".mzr-content").mouseout(function () {
    $(this).hide();
});
$(".submenu").mouseover(function () {
    $(this).siblings('.mzr-content').show();
});
$(".mzr-content").mouseover(function () {
    $(this).show();
});
// ============ end tile for <a> and alt for img ========



$(document).ready(function () {
    $(".slide-panel-button").click(function () {
        $('#domain-selector-panel').toggle()//animate({height: "toggle"}, 200);
        if ($('#domain-selector-panel').is(":visible")) {
            $('#div-lang').css('z-index', '200');
        } else {
            $('#div-lang').css('z-index', '1001');
        }
        $(".slide-panel-button").toggle();
    });

});






<!-- ============ start accessibility megamenu ============ -->



$(document).ready(function ($) {


    $("#dawgdrops").accessibleMegaMenu({
        /* prefix for generated unique id attributes, which are required
         to indicate aria-owns, aria-controls and aria-labelledby */
        uuidPrefix: "accessible-megamenu",

        /* css class used to define the megamenu styling */
        menuClass: "meganizr",

        /* css class for a top-level navigation item in the megamenu */
        topNavItemClass: "mzr-drop",

        /* css class for a megamenu panel */
        panelClass: "mzr-content",

        /* css class for a group of items within a megamenu panel */
        panelGroupClass: "mzr-links",

        /* css class for the hover state */
        hoverClass: "hover",

        /* css class for the focus state */
        focusClass: "focus-content",

        /* css class for the open state */
        openClass: "open hover-content"
    });

});





$(document).ready(function () {
    var wi = $(window).width();
    if (wi < 769) {
        $('#printable_area td').removeAttr('style');
        $('#printable_area td p').css("width", "100%");
    }
});
$( ".meganizr .mzr-drop").keyup(function() {

    $(".mzr-content").attr("aria-hidden","true");
    $(this).find(".mzr-content").attr("aria-hidden", "false");
});
// ============ start tile for <a> and alt for img ========
$('a').each(function() {
    $(this).attr('title', $(this).text());
});

var lan = "bn";
if(lan == 'en'){
    $('.mzr-drop a:first-child').each(function() {
        $(this).attr('title', "Enter to get more menu");
    });
}else{
    $('.mzr-drop a:first-child').each(function() {
        $(this).attr('title', "Menu");
    });
}
$('img').each(function() {
    var str = $(this).attr("src");
    var arr = str.split('/');
    var strFine = arr[arr.length-1];

    var str2 = strFine;
    var arr2 = str2.split('.');
    var strFine2 = arr2[arr2.length-2];
    $(this).attr('alt', strFine2);
});
$('a2iLogo').attr('alt', 'Access to information');
$('.service-box img').each(function() {
    var strTitle = $(this).parent().find('h4').text();
    $(this).attr('alt', strTitle);
});
$('.block img').each(function() {
    var strTitleRight = $(this).closest('.block').find('.title').text();
    $(this).attr('alt', strTitleRight);
});
$('#notice-board-ticker .btn').attr('title', 'à¦¸à¦•à¦² à¦¨à§‹à¦Ÿà¦¿à¦¶');
$('#news-ticker .btn').attr('title', 'à¦¸à¦•à¦² à¦–à¦¬à¦°');
$('#search').each(function() {
    $(this).attr('alt', 'Search');
});
$('.search-btn').each(function() {
    $(this).attr('alt', 'Enter to search');
});
// ============ end tile for <a> and alt for img ========

// =============== start dropdown design =======
$(".mzr-content").mouseout(function(){
    // $(this).hide();
});
$(".submenu").mouseover(function(){
    //$('.mzr-content').show();
});
$(".mzr-content").mouseover(function(){
    //$(this).show();
});
// =============== End dropdown design =======
