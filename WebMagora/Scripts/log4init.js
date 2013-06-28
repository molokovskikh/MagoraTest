//обертка для логгера
var logger = function (top) {
    var log = window.log;
    if (log) {
        if (log.ancor) {
            if (!log.ancor.isVisible()) {
                if (log.ancor.timer_fn) log.ancor.timer_fn();
                log.ancor.show();
            }
            if (top) log.ancor.setFocusPopUp(true);
        }
        return log;
    }
    //if(!$.browser.msie){debugger} 
    return {
        info: function () { },
        debug: function () { },
        error: function () { },
        trace: function () { }
    };

}

//перенаправим ошибки в логгер
$(window).on('onerror',
    function (errorMsg, url, lineNumber) 
    {
        logger().error('javascriptError:'+errorMsg+"|line:"+lineNumber+"|url"+lineNumber);
        return false;
    });

//регистрируем ошибки Ajax
$(document).ajaxError(function (event, jqXHR, ajaxSettings, thrownError)
{
   //Получим исключение возникшее на веб-сервере
    var _getServerException = function (data,r) {
        if (data&&typeof (data) == 'object') {
            var _data = data;
            if (data.responseText && typeof (data.responseText) == 'string' && data.responseText.length>0 && $.parseJSON)
            {
                try
                { _data = $.parseJSON(data.responseText); }
                catch (e_)
                { }
            }

            if (_data&&_data.Exception&&_data.Exception.Type) {
                    return _data.Exception;
                }
            if(r) return _data;           
        }
        return null;
    };

    var exc_server = _getServerException(jqXHR);     //Получим ошибку с сервера
    if(exc_server) //Если серверное исключение, тогда регистрируем
        logger().error('ajaxServerError:' + ajaxSettings.url + '|' + exc_server, thrownError);
    else 
       logger().error('ajaxError:' + ajaxSettings.url + '|' + jqXHR.status, thrownError);
});


