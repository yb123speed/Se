
/*
* 微信分享6.0.2新版 API
* victor
*/

//***********LOAD SDK****************
var DOMAINX = "http://usp.zhuchao.cc";
var LOADWXJS = document.createElement("script");
LOADWXJS.setAttribute("src", "http://res.wx.qq.com/open/js/jweixin-1.0.0.js");
document.head.appendChild(LOADWXJS);

//***********SDK配置*******************

var WXJSSDK = WXJSSDK || {};
var WXAPI = WXAPI || {};
WXJSSDK.config = {
    debug: false, // 调试
    appId: "wx615248120f7422aa", //'wxcaebd5318cdb7fa1'正式的   wxe27a199a801aab0f 测试的，公众号的唯一标识,如果提示签名无效需检查此id是否和当前绑定的js安全域名同一个公众号
    timestamp: parseInt((new Date).getTime() / 1000), // 必填，生成签名的时间戳
    nonceStr: Math.random().toString(36).substr(2), // 必填，生成签名的随机串
    signature: '', // 签名
    // 必填，需要使用的JS接口列表
    jsApiList: ['checkJsApi',
                'onMenuShareTimeline',
                'onMenuShareAppMessage',
                'onMenuShareQQ',
                'onMenuShareWeibo',
                'hideMenuItems',
                'showMenuItems',
                'hideAllNonBaseMenuItem',
                'showAllNonBaseMenuItem',
                'translateVoice',
                'startRecord',
                'stopRecord',
                'onRecordEnd',
                'playVoice',
                'pauseVoice',
                'stopVoice',
                'uploadVoice',
                'downloadVoice',
                'chooseImage',
                'previewImage',
                'uploadImage',
                'downloadImage',
                'getNetworkType',
                'openLocation',
                'getLocation',
                'hideOptionMenu',
                'showOptionMenu',
                'closeWindow',
                'scanQRCode',
                'openProductSpecificView',
                'addCard',
                'chooseCard',
                'openCard']
};

WXJSSDK.signature = function (callback) {
    $.ajax({
        url: DOMAINX + "/Mobile/ashx/Signature.ashx",
        type: "POST",
        data: { timestamp: WXJSSDK.config.timestamp, nonceStr: WXJSSDK.config.nonceStr, url: window.location.href.split('#')[0] },

        success: function (data) {
            WXJSSDK.config.signature = data;
            callback && callback();
        }
    });
};

// 加载微信开发工具包 
// callbacks： 是一个包含了各种回调的对象
WXJSSDK.load = function (callbacks) {

    //SDK加载完成后
    LOADWXJS.onload = function () {
        //注入配置信息
        WXJSSDK.signature(function () {
            wx.config(WXJSSDK.config);
        });
        //config信息验证后会执行ready方法
        wx.ready(function () {
            callbacks.ready && callbacks.ready();
        });
        //        // 接口调用成功时执行的回调函数
        //        wx.success(function (res) {
        //            callbacks.success && callbacks.success();
        //        });
        //        // 接口调用失败时执行的回调函数
        //        wx.fail(function (res) {
        //            $.vAlert ? $.vAlert(res.errMsg) : alert(res.errMsg);
        //            callbacks.fail && callbacks.fail();
        //        });
        //        // 接口调用完成时执行的回调函数，无论成功或失败都会执行
        //        wx.complete(function (res) {
        //            callbacks.complete && callbacks.complete();
        //        });
        //        // 用户点击取消时的回调函数，仅部分有用户取消操作的api才会用到
        //        wx.cancel(function (res) {
        //            callbacks.cancel && callbacks.cancel();
        //        });
        //        // 监听Menu中的按钮点击时触发的方法，该方法仅支持Menu中的相关接口
        //        wx.trigger(function (res) {
        //            callbacks.trigger && callbacks.trigger();
        //        });
        // config信息验证失败会执行error函数
        wx.error(function (res) {
            $.flytip ? $.flytip(res.errMsg) : alert(res.errMsg);
            callbacks.error && callbacks.error();
        });
    };

};


//*************API定义*****************

//==============================
// 微信分享接口
//==============================
WXAPI.shares = function (params) {
    wx.showOptionMenu();

    // 分享到朋友圈
    wx.onMenuShareTimeline({
        title: params.title, // 分享标题
        link: params.shareCircle ? params.shareCircle : params.link, // 分享链接
        imgUrl: params.imgUrl, // 分享图标
        success: function () {
            params.timelineSuccess ? params.timelineSuccess() : params.success && params.success();
        },
        cancel: function () {
            params.timelineCancel ? params.timelineCancel() : params.cancel && params.cancel();
        }
    });

    // 分享给朋友
    wx.onMenuShareAppMessage({
        title: params.title, // 分享标题
        desc: params.desc, // 分享描述
        link: params.shareFriend ? params.shareFriend : params.link, // 分享链接
        imgUrl: params.imgUrl, // 分享图标
        type: params.type ? params.type : "link", // 分享类型,music、video或link，不填默认为link
        dataUrl: params.dataUrl ? params.dataUrl : "", // 如果type是music或video，则要提供数据链接，默认为空
        success: function () {
            params.appMessageSuccess ? params.appMessageSuccess() : params.success && params.success();
        },
        cancel: function () {
            params.appMessageCancel ? params.appMessageCancel() : params.cancel && params.cancel();
        }
    });

    // 分享到QQ
    wx.onMenuShareQQ({
        title: params.title, // 分享标题
        desc: params.desc, // 分享描述
        link: params.shareQQ ? params.shareQQ : params.link, // 分享链接
        imgUrl: params.imgUrl, // 分享图标
        success: function () {
            params.shareQQSuccess ? params.shareQQSuccess() : params.success && params.success();
        },
        cancel: function () {
            params.shareQQCancel ? params.shareQQCancel() : params.cancel && params.cancel();
        }
    });

    // 分享到腾讯微博
    wx.onMenuShareWeibo({
        title: params.title, // 分享标题
        desc: params.desc, // 分享描述
        link: params.shareWeibo ? params.shareWeibo : params.link, // 分享链接
        imgUrl: params.imgUrl, // 分享图标
        success: function () {
            params.shareWeiboSuccess ? params.shareWeiboSuccess() : params.success && params.success();
        },
        cancel: function () {
            params.shareWeiboCancel ? params.shareWeiboCancel() : params.cancel && params.cancel();
        }
    });
};

//==============================
//微信上传图片接口
//==============================
WXAPI.img = {
    //坑爹的微信没有提供限制选择图片数量的接口,用户可以一次最多选择9个图片
    //需要提供参数num：上传数量，超过设定的我忽略掉不上传哈！
    images: {
        localId: [],
        serverId: [],
        num: null,
        fileType: "weixin",
        sizeRange: null
    },
    //选择图片
    choose: function (callback) {
        var me = WXAPI.img;

        me.images.localId = [];
        me.images.serverId = [];

        wx.chooseImage({
            success: function (res) {
                me.images.localId = res.localIds;
                callback && callback();
            }
        });
    },
    //执行上传
    upload: function (callback) {
        var me = WXAPI.img;
        var i = 0,
			length = me.images.num ? me.images.num : me.images.localId.length;
        var $loading = $("#loading"),
			hasLoading = $loading.length;

        function dowloadImg(dcallback) {
            $.ajax({
                url: DOMAINX + "/ashx/downimage.ashx?serverId=" + me.images.serverId,
                type: "GET",
                //data: {fileType: me.images.fileType, mediaId: me.images.serverId, sizeRange: me.images.sizeRange},
                //dataType: "json",
                beforeSend: function () {
                    hasLoading && $loading.show();
                },
                success: function (data) {
                    if (data) {

                        hasLoading && $loading.hide();

                        //var msg = "上传成功";
                        //$.flytip ? $.flytip(msg) : alert(msg);
                        //返回的data是一个数组，数组的元素是对象，包含了图片url,path，并返回给回调函数
                        callback && callback(data);
                        dcallback();
                    }

                }
            });
        }
        function uploadImg() {
            if (i >= length) {
                $.flytip && $.flytip("上传完毕");
                return false;
            }
            wx.uploadImage({
                localId: me.images.localId[i], // 需要上传的图片的本地ID，由chooseImage接口获得
                isShowProgressTips: 1, // 默认为1，显示进度提示
                success: function (res) {

                    me.images.serverId = res.serverId;

                    if (i < length) {
                        dowloadImg(function () {
                            uploadImg();
                        });

                    }

                    i++;

                },
                fail: function (res) {
                    var msg = JSON.stringify(res);
                    $.flytip ? $.flytip(msg) : alert(msg);
                }
            });
        }

        uploadImg();
    },
    //查看图片
    preview: function (imgcur, imgurls) {
        wx.previewImage({
            current: imgcur, // 当前显示的图片链接
            urls: imgurls // 数组，需要预览的图片链接列表
        });
    }
};

