/*

validate="{min:{v:123,msg:'hello'},max{v:321,msg:'test'},code{v:}}"

如不需失去焦点验证 为 element 或者Form 增加 属性或attr "noBlurValid"  如 $('#name')[0].noBlurValid = true; $('form')[0].noBlurValid = true; 或直接写在html中

*/
var validBool = false;
//validate
function validate() {
    var fs = $('form:visible');
    // var fs = $('form');

    //清理验证信息
    function ecm(obj) {
        $(obj).removeClass('valid').removeClass('valid-err').removeClass('valid-testing');
        (obj.form.validClear != undefined ? obj.form.validClear : window.validClear != undefined ? window.validClear : function () { })(obj);
    }


    //正在验证
    function evn(obj) {
        ecm(obj);
        $(obj).addClass('valid-testing');
        (obj.form.validChecking != undefined ? obj.form.validChecking : window.validChecking != undefined ? window.validChecking : function () { })(obj);
        return false;
    }

    //验证失败事件 用来显示界面  obj = 验证对象, v = json
    function eem(obj, v) {
        //ecm(obj);
        $(obj).addClass('valid-err');
        var msg = v.msg.replace('{n}', obj.name).replace('{v}', v.v);
        if (msg.indexOf('{v0}') > 0 || msg.indexOf('{v1}') > 0) {
            msg = msg.replace('{v0}', v.v[0]).replace('{v1}', v.v[1]);
        }
        (obj.form.validFail != undefined ? obj.form.validFail : window.validFail != undefined ? window.validFail : function (obj, msg) { layer.tips(msg, obj, { guide: 1, time: 2 }) })(obj, msg);
        //validBool = false;
        return false;
    };

    //验证成功
    function evm(obj) {
        ecm(obj);
        $(obj).addClass('valid');
        (obj.form.validSucceed != undefined ? obj.form.validSucceed : window.validSucceed != undefined ? window.validSucceed : function () { })(obj);
        //validBool = true;
        return true;
    }

    //验证控件 第一次绑定
    function _valid(_obj) {
        //如果元素没有验证方法 
        if (_obj.validate == undefined) {
            //为元素新增规则 参数 是否同步刷新  同步刷新为强制刷新,用于取消焦点事件. 非同步直接返回之前的结果,用于表单提交.
            _obj.validate = function (isSync) {
                //如果不需要强制刷新
                var c = $(this);
                var vd = c.attr('valid');
                // alert(vd);
                if (!isSync) {
                    if (!vd) {
                        return true;
                    }
                    if (c.hasClass('valid')) {
                        return true;
                    }
                    if (c.hasClass('valid-err')) {
                        return false;
                    }
                    if (c.hasClass('valid-testing')) {
                        return false;
                    }
                }
                //alert(vd);
                if (vd == undefined) {
                    return true; // evm(this);
                }

                var js = eval('(' + vd + ')');
                //alert(js);
                var val = this.value;
                var valn = parseFloat(val);

                //不能为空 
                var v = js.request;
                if (v != undefined && val == '' || val == '请输入名字' || val == '请输入验证码' || val == '请输入密码')
                    return eem(this, { v: v.v, msg: v.m ? v.m : '需要填点东西,不能为空!' });
                //如果为空,则通过
                if (val == '') {
                    return evm(this);
                }

                //最小值
                v = js.min;
                if (v != undefined && (valn == NaN || valn < v.v))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '最小不能低于{v}!' });

                //最大值
                v = js.max;
                if (v != undefined && (valn == NaN || valn > v.v))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '最大不能超过{v}!' });
                //用户名字母开头
                v = js.username;

                if (_obj.id == "iptUserName") {
                    if (val.length <= 3) {
                        return eem(this, { v: v.v, msg: v.m ? v.m : '用户名长度不能小于4位' });
                    }
                    else {
                    }
                }
                else if (v != undefined && !/^[a-zA-Z]{1}[0-9a-zA-Z_]{2,}$/.test(val)) {
                    return eem(this, { v: v.v, msg: v.m ? v.m : '须以字母开头，使用字母、数字、下划线' });
                }
                //密码一致
                v = js.password;
                //alert($('#' + v).val());
                if (v != undefined && val != v.v)
                    return eem(this, { v: v.v, msg: v.m ? v.m : '两次密码输入不一致' });
                //范围
                v = js.range;
                if (v != undefined && (valn == NaN || valn < v.v[0] || valn > v.v[1]))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入一个在 {v0} 和 {v1} 之间的值!' });

                //字符串长度范围
                v = js.range_len;
                if (v != undefined && (val.length < v.v[0] || val.length > v.v[1]))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入 {v0} 到 {v1} 个字符!' });

                //字符串长度范围
                v = js.len;
                if (v != undefined && (val.length != v.v))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入 {v} 个字符!' });

                //最少多少位
                v = js.minlen;
                if (v != undefined && (val.length < v.v))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请至少输入 {v} 个字符!' });
                //Email 
                v = js.email;
                if (v != undefined && !/^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(val))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入格式正确的邮箱地址!' });

                //Mobile
                v = js.mobile;
                if (v != undefined && !/^1[3|4|5|7|8][0-9]\d{8}$/.test(val))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入格式正确的手机号码!' });
                v = js.phone;

                if (v != undefined && !/^0\d{2,3}-?\d{7,8}$/.test(val))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入格式正确的座机号码!' });
                //Equals
                v = js.equals;
                if (v != undefined && (v.v.substr(0, 1) == '#' ? $(v.v).val() : v.v) != val)
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入指定的值!' });

                //整数
                v = js.int;
                if (v != undefined && !/^-?\d*$/.test(val))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入整数数字!' });

                //浮点数
                v = js.float;
                if (v != undefined && !/^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test(val))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入数字!' });

                //时间
                v = js.datetime;
                if (v != undefined && !/^(\d{4})-(\d{2})-(\d{2})$/.test(val))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '请输入正确的时间格式!' });

                //正则
                v = js.test;
                if (v != undefined && !(new RegExp(v.v).test(val)))
                    return eem(this, { v: v.v, msg: v.m ? v.m : '内容验证未通过!' });

                //远端验证
                v = js.code;
                if (v != undefined) {
                    evn(this);
                    //v.v = '/Code/' + v.v;
                    var data = $(this.form).serializeArray();
                    data.push({ name: 'n', value: this.name });
                    data.push({ name: 'v', value: this.value });
                    data.push({ name: '__t', value: 3328 });
                    //如果是同步刷新,是取消焦点事件,不需要同步Ajax,
                    if (!isSync) {
                        var art = $.ajax({
                            url: v.v,
                            data: data,
                            type: "POST",
                            context: this,
                            async: false
                        });

                        if (art.status != 200) {
                            tip('服务端返回' + art.status + '错误.', 'error');
                            throw '服务端返回' + art.status + '错误.';
                        }
                        art = art.responseText;
                        if (art == "True") {
                            //验证全部通过
                            return evm(this);
                        }
                        else {
                            //验证未通过
                            if (art.substr(0, 2) == '<!') {
                                tip('权限丢失,尝试重新建立连接.', function () { reload(); });
                                return false;
                            }

                            return eem(this, { v: art, msg: v.msg = art != 'False' ? art : '远端验证失败!' });
                        }
                    }
                    else {
                        $.ajax({
                            url: v.v,
                            data: data,
                            type: "POST",
                            context: this,
                            success: function (rt) {
                                if (rt == "True") {
                                    //验证全部通过
                                    evm(this);
                                }
                                else {
                                    //验证未通过 
                                    if (rt.substr(0, 2) == '<!') {
                                        tip('权限丢失,尝试重新建立连接.', function () { reload(); });
                                        return false;
                                    }

                                    eem(this, { v: rt, msg: v.msg = rt != 'False' ? rt : '远端验证失败!' });
                                }
                            },
                            error: function (rt) {
                                if (rt.status == 0) return; tip('服务端返回' + rt.status + '错误.', 'error');
                            }
                        });
                    }

                    //正在校验
                    return evn(this);
                }
                else {
                    //验证通过
                    return evm(this);
                }
            };

            //失去焦点,验证
            //            $(_obj).blur(function () {
            //                var isNeedValid = (this.noBlurValid || $(this).attr('noBlurValid') || this.form.noBlurValid || $(this.form).attr('noBlurValid'));
            //                if (isNeedValid) {
            //                    return;
            //                }
            //                else {
            //                    this.validate(true);
            //                }
            //            });
            $(_obj).blur(function () {
                this.validate(true);
            });

            $(_obj).focus(function () { ecm(this); });
            return true;
        }
        else {
            return _obj.validate();
        }
    }

    //遍历所有表单
    fs.each(function () {
        var f = this;
        //adminValid();

        //为表单增加Validate方法
        f.validate = function () {
            var ips = this.elements;

            var isValidate = true;

            //遍历input
            for (var i = 0; i < ips.length; i++) {
                ecm(ips[i]);
                isValidate &= _valid(ips[i]);
                if (!_valid(ips[i])) {
                    return false;
                }
                //alert(isValidate);
            }
            //alert(isValidate);
            if (isValidate == 0) {
                validBool = false;
            } else {
                validBool = true;
            }
            return isValidate == 0 ? false : true;
        };

        //绑定表单中元素的验证方法
        f.validate();

        //为表单提交增加验证方法
        $(f).submit(function () {
            return f.validate();
        });
    });
    return validBool;
}

//自动验证
//$(validate);
//设置验证表单的验证样式和方式
function adminValid() {
    window.validClear = function (obj) {
        //layer.tips("这是一个测试信息", $("#txtTip"), { guide: 1, time: 2 });

        //$(obj).next('.valid_info').remove();
    };
    window.validFail = function (obj, msg) {
        var a = $(obj).attr("id");
        if (a == "user_name" || a == "pass_word" || a == "check_code") {
            layer.tips(msg, obj, { guide: 2, time: 2 });
        }
        else {
            layer.tips(msg, obj, { guide: 1, time: 2 });
        }
        //$(obj).after('<span class="valid_info c_red" style="color:red">' + msg + '</span>');
        //$(obj).after('<span class="regin_info regin_info_false fl ml10"><i></i><b class="fl ml10">' + msg + '</b></span>');
    };
}
$(adminValid);


