(function($){ 
    'use strict';

    // document ready begin
    $(document).ready(function() {
    
        // Event Listeners
        (function (){
            var $jsToggle               = $(".js-toggle"),
                $jsToggleAll            = $(".js-all-toggle"),
                $jsPrint                = $(".js-print"),
                $jsBackToTop            = $(".js-back-to-top"),
                $jsLegend               = $(".js-legend"),
                $legend                 = $(".legend"),
                _isBarsExpanded         = false,
                _isLegendShown          = false,
                mY                      = 0,
                dir                     = "neutral";

            /*
             * Expand / Collapse Bar
             */
            $jsToggle.bind("click", function (evt){
                evt.preventDefault();

                var newHover = e.relatedTarget;
                if( newHover === $jsToggle[0] )           return;                
                if( $.contains($jsToggle[0], newHover) )  return;
            });

            
            /*
             * Expand / Collapse All Bars
             */
            $jsToggleAll.on("click", function (evt){
                evt.preventDefault();

                var _this = $(this);

                if ( !_isBarsExpanded ) {
                    _this.find("span").text("Collapse All");

                    $(".details, .comments, .note.asterisk").slideDown();
                    $(".bar")
                        .addClass("active")
                        .css({ "zoom" : 0 });

                    setTimeout(function(){ $(".bar").css({ "zoom" : 1 }) }, 200);
                } else {
                    _this.find("span").text("Expand All");

                    $(".details, .comments, .note.asterisk").slideUp();
                    $(".bar")
                        .removeClass("active")
                        .css({ "zoom" : 0 });

                    setTimeout(function(){ $(".bar").css({ "zoom" : 1 }) }, 200);
                }

                _isBarsExpanded = !_isBarsExpanded;
            });


            /*
             * Print Page
             */
            $jsPrint.on("click", function (evt){
                evt.preventDefault();
                window.print();
            });


            /*
             * Back To Top
             */
            $jsBackToTop.on("click", function (evt){
                evt.preventDefault();
                $('html,body').animate({ scrollTop: 0 });
            });


            /*
             * Show / Hide Legend
             */
            $jsLegend.bind("mouseover", function (e){
                if (!_isLegendShown)    $legend.slideDown();
            }).bind("click", function (){
                if (!_isLegendShown)    $legend.slideDown();
                else                    $legend.slideUp();

                _isLegendShown = !_isLegendShown;
            });


            /*
             * Legend Animation Fix
             */            
            $legend.bind("mouseout", function (e){
                var newHover = e.relatedTarget;
                if( newHover === $legend[0] )           return;                
                if( $.contains($legend[0], newHover) )  return;                

                if ( !_isLegendShown && dir == "down" )     $(this).slideUp();
            }).bind("mousemove", function (e){
                if (e.pageY < mY)   dir = "up";
                else                dir = "down";
                
                mY = e.pageY;
            });
        })();


        // Content Population
        (function (){
            var loc             = "",
                $secondaryNav   = $("#secondary-nav"),
                $main           = $("#main"),
                $results        = $(".results"),
                $about          = $(".about"),
                $btnAbout       = $(".btn-about"),
                jsonObj;


            $secondaryNav.on("click", "a", function(e){
                e.preventDefault();

                var $this   = $(this).parent(),
                    _ind    = $this.index();

                $results.slideDown();
                $about.slideUp();

                if ( !$this.hasClass("active") ){
                    $secondaryNav.find(".active").removeClass("active");
                    $this.addClass("active");
                    $main.removeClass();

                    switch (_ind){
                        case 0:                                        break;
                        case 1:  $main.addClass("inside strategy");    break;
                        case 2:  $main.addClass("inside social");      break;
                        case 3:  $main.addClass("inside web");         break;
                    }
                    
                    loadJson();
                } 
            });

            $btnAbout.on("click", function (e){
                e.preventDefault();

                $secondaryNav.find(".active").removeClass("active");
                $main.removeClass();
                $results.slideUp();
                $about.slideDown();
            });
            
            // Determine if Home Page or Inside Page for JSON Loading
            function loadJson(){                
                $(".btn-bar").off("click");


                if ( $(".inside").length ) {

                    // Strategy / Social Media / Web
                    loc = "inside";

                    if ( $(".strategy").length )       jsonObj = strategy;
                    else if ( $(".social").length )    jsonObj = social_media;
                    else if ( $(".web").length )       jsonObj = web;

                    populateData(jsonObj, loc);
                } else {

                    // Home ONLY
                    loc         = "home";
                    jsonObj     = home;

                    if ( !$(".static").length ){
                        populateData(jsonObj, loc);
                    } else {
                        enableBars();
                    }
                }
            };


            /*
             * Populate Data from JSON
             */
            function populateData(json, loc){
                var introData   = "",
                    injectData  = "";            

                introData += '<h1>' + json.title + '</h1>';
                introData += '<p>' + json.description + '</p>';

                for ( var i = 0; i < json.results.length; i++ ){
                    injectData += '<div class="bar lvl1-bar" data-rate="' + json.results[i].score + '" data-link="' + json.results[i].link + '" >';
                    injectData += '<div class="btn-bar">';
                    injectData += '<h2>' + json.results[i].title + '</h2>';
                    injectData += '<div class="control">';
                    injectData += '<span class="rate">' + json.results[i].score + '</span>';
                    
                    // Display Chevron or Plus/Minus Icon
                    if ( loc == "inside" )  injectData += '<button class="icon-toggle js-toggle"><span class="visuallyhidden">Toggle</span></button>';
                    else                    injectData += '<button class="js-toggle"><i class="icon icon-chevron"></i></button>';

                    injectData += '</div>';

                    // Display Info under header
                    if ( json.results[i].description !== "" ) {                        
                        injectData += '<div class="note">';
                        injectData += '<i class=\"icon icon-info\"></i><p>' + json.results[i].description + '</p>';
                        injectData += '</div>';
                    }

                    injectData += '</div>';
                    injectData += '</div>';

                    // Display Level 2
                    if ( json.results[i].details_level_1 !== "" ){
                        injectData += '<div class="details">';
                        injectData += '<div class="wrap">';

                        for ( var j = 0; j < json.results[i].details_level_1.length; j++ ) {
                            injectData += '<div class="bar lvl2-bar" data-rate="' + json.results[i].details_level_1[j].score + '">';
                            injectData += '<div class="btn-bar">';
                            injectData += '<h2>' + json.results[i].details_level_1[j].title + '</h2>';
                            injectData += '<div class="control">';
                            injectData += '<span class="rate">' + json.results[i].details_level_1[j].score + '</span>';
                            
                            injectData += '<button class="icon-toggle js-toggle">';
                            injectData += '<span class="visuallyhidden">Toggle</span>';
                            injectData += '</button>';

                            injectData += '</div>';
                            injectData += '</div>';

                            injectData += '<div class="note">';
                            
                            // Display Info under header
                            if ( json.results[i].details_level_1[j].description !== "" )
                                injectData += '<i class=\"icon icon-info\"></i><p>' + json.results[i].details_level_1[j].description + '</p>';                            

                            if ( json.results[i].details_level_1[j].details_level_2 == "" ) {
                                injectData += '<div class="comments">';
                                injectData += '<div class="wrap first">';                                

                                if ( json.results[i].details_level_1[j].indicator_for !== undefined ){

                                    // Display Highscore if present
                                    injectData += '<div class="col col-6 first">';
                                    injectData += '<h4>Criteria</h4>';
                                    injectData += '<p>' + json.results[i].details_level_1[j].criteria + '</p>';
                                    injectData += '</div>';

                                    injectData += '<div class="col col-6 last">';
                                    injectData += '<h4>A high score indicates</h4>';
                                    injectData += '<p>' + json.results[i].details_level_1[j].indicator_for + '</p>';
                                    injectData += '</div>';
                                } else {

                                    // Hide Highscore if ""
                                    injectData += '<div class="col col-12 only last">';
                                    injectData += '<h4>Criteria</h4>';                                    
                                    injectData += '<p>' + json.results[i].details_level_1[j].criteria + '</p>';
                                    injectData += '</div>';
                                }

                                injectData += '</div>';
                                
                                injectData += '<div class="wrap last">';
                                injectData += '<h4>Recommendations</h4>';
                                injectData += '<p>' + json.results[i].details_level_1[j].recommendations + '</p>';
                                
                                // Display Assessor's Comments if present
                                if ( json.results[i].details_level_1[j].assessor !== undefined ){
                                    injectData += '<hr/>';
                                    injectData += '<h4>Assessor\'s Comments</h4>';
                                    injectData += '<p>' + json.results[i].details_level_1[j].assessor + '</p>';
                                }
                                
                                injectData += '</div>';
                                injectData += '</div>';
                            }
                            
                            injectData += '</div>';                            
                            injectData += '</div>';
                            
                            // Display Level 3
                            if ( json.results[i].details_level_1[j].details_level_2 != "" ) {
                                injectData += '<div class="details">';
                                
                                for ( var k = 0; k < json.results[i].details_level_1[j].details_level_2.length; k++ ) {
                                    injectData += '<div class="bar lvl3-bar" data-rate="' + json.results[i].details_level_1[j].details_level_2[k].score + '">';                                
                                    injectData += '<div class="btn-bar">';
                                    injectData += '<h3>' + json.results[i].details_level_1[j].details_level_2[k].title + "</h3>";
                                    injectData += '<div class="control">';
                                    injectData += '<span class="rate">' + json.results[i].details_level_1[j].details_level_2[k].score + '</span>';
                                    injectData += '<button class="icon-toggle js-toggle">';
                                    injectData += '<span class="visuallyhidden">Toggle</span>';
                                    injectData += '</button>';
                                    injectData += '</div>';
                                    injectData += '</div>';
                                    injectData += '<div class="note">';

                                    // Display Info under header
                                    if ( json.results[i].details_level_1[j].details_level_2[k].description !== "" )
                                        injectData += '<i class=\"icon icon-info\"></i><p>' + json.results[i].details_level_1[j].details_level_2[k].description + '</p>';

                                    injectData += '<div class="comments">';
                                    injectData += '<div class="wrap first">';

                                    injectData += '<div class="col col-6 first">';
                                    injectData += '<h4>Criteria</h4>';
                                    injectData += '<p>' + json.results[i].details_level_1[j].details_level_2[k].criteria + '</p>';
                                    injectData += '</div>';

                                    injectData += '<div class="col col-6 last">';
                                    injectData += '<h4>A high score indicates</h4>';
                                    injectData += '<p>' + json.results[i].details_level_1[j].details_level_2[k].indicator_for + '</p>';
                                    injectData += '</div>';

                                    injectData += '</div>';

                                    injectData += '<div class="wrap last">';
                                    injectData += '<h4>Recommendations</h4>';
                                    injectData += '<p>' + json.results[i].details_level_1[j].details_level_2[k].recommendations + '</p>';
                                    
                                    // Display Assessor's Comments if present
                                    if (json.results[i].details_level_1[j].details_level_2[k].assessor !== "" ){
                                        injectData += '<hr/>';
                                        injectData += '<h4>Assessor\'s Comments</h4>';
                                        injectData += '<p>' + json.results[i].details_level_1[j].details_level_2[k].assessor + '</p>';
                                    }
                                    
                                    injectData += '</div>';
                                    injectData += '</div>';
                                    injectData += '</div>';
                                    injectData += '</div>';                                
                                }

                                injectData += '</div>';
                            }
                        }
                        injectData += '</div>';
                        injectData += '</div>';
                    }
                }

                // Insert Intro Markup
                $(".intro .wrap").empty().html( introData );

                // Insert Results Markup
                $(".results .cont").empty().html( injectData );

                enableBars();            


                /*
                 * Assign colors to bars depending on 0-10 rating             
                 * Enable Bars Event Listeners
                 */
                function enableBars(){
                    var $bar                = $(".btn-bar"),
                        $body               = $("body"),
                        eventType           = "",
                        mX                  = 0,
                        mY                  = 0,
                        mouseSensitivity    = 2;


                    // Assign colors to bars depending on 0-10 rating
                    $bar.each(function (){
                        var _this   = $(this).parent(),
                            rate    = Math.floor( _this.data("rate") );
                        
                        if ( _this.data("rate") !== "NA" )      _this.addClass( "spectrum" + rate );
                        else                                    _this.addClass( "spectrumNA" );
                    }); 


                    // Enable Bars Event Listeners
                    $bar.on("click", function (){
                        var _this = $(this).parent();
                        
                        if ( _this.hasClass("active") )     _this.removeClass("active");
                        else                                _this.addClass("active");

                        if ( _this.data("link") !== null && _this.data("link") !== undefined && _this.data("link") !== "" ) {
                            // window.location.href = _this.data("link");                                                        
                            $secondaryNav.find("li:nth-child(" + _this.data("link") + ") a").trigger("click");
                        } else {
                            if ( _this.hasClass("lvl3-bar") ){
                                _this.find(".comments").slideToggle(function (){ _this.css({ "zoom" : 0 }) });
                            } else {
                                if ( _this.hasClass("lvl1-bar") ){
                                    if ( $(".lvl2-bar").length ) {
                                        _this.next().slideToggle(function (){ _this.css({ "zoom" : 0 }) });
                                    } else {
                                        _this.find(".comments").slideToggle(function (){ _this.css({ "zoom" : 0 }) });
                                    }
                                } else {                       
                                    if ( $(".lvl3-bar").length ){
                                        _this.next().slideToggle(function (){ _this.css({ "zoom" : 0 }) });
                                    } else {
                                        _this.find(".comments").slideToggle(function (){ _this.css({ "zoom" : 0 }) });
                                    }
                                }
                            }
                            setTimeout(function(){ _this.css({ "zoom" : 1 }) }, 100);
                        }                    
                    });

                }
            };


            // Trigger Landing Page
            $secondaryNav.find(".first a").trigger("click");
        })(); 


        // Dashboard
        (function (){
            var pickerFrom, 
                pickerTo;

            pickerFrom = new Pikaday({ 
                field : document.getElementById('inputDateFrom'),
                onSelect : function(date){
                    var set_date = moment(date).add(3, 'months');
                    pickerTo.setMoment(set_date, true);
                }
            });

            pickerTo = new Pikaday({ 
                field: document.getElementById('inputDateTo'),
                onSelect: function(date){
                    var set_date = moment(date).subtract(3, 'months');
                    pickerFrom.setMoment(set_date, true);
                }
            });
        })();

        var $dashboard = $('.dashboard');

        $('.js-assessment-chkbx', $dashboard).click(function(e){
            var $this = $(this),
                $input = $this.parent().parent().find('.input-txt');
                

            $input.val('');
            if($input.prop('disabled')==true){
               $input.prop('disabled',false); 
            }else{
                $input.prop('disabled',true); 
            }            
        });

        $('.js-input-audience', $dashboard).keyup(function(e){
            var $this = $(this);
            if( $this.val().trim()=='') {                
                $('.js-input-scenario', $this.parent()).val('');
                $('.js-input-scenario', $this.parent()).prop("disabled", true);
            }else{
                $('.js-input-scenario', $this.parent()).prop("disabled", false);
            }
        });

        $('.js-load-social-data', $dashboard).click(function(e){
            var $this = $(this);
            $this.addClass('is-loading');
            $this.find('.label').html('Loading...');
            $this.prop("disabled", true);
            setTimeout(function(){ 
                var $download_link = $this.parent().find('.cta');
                $this.hide();
                $download_link.fadeIn();
            }, 3000);
        });


        /*
         * Assessment Control Toggle (Dashboard)
         */
        var $loadAssessment = $(".js-load-assessment"),
            $assessmentList = $(".assessment-list"),
            emptyField      = 0;

        $loadAssessment.on("click", function (){
            $assessmentList.find("input").each(function (){
                if ( $(this).val() )
                    emptyField += 1;

                console.log(emptyField);

                if ( emptyField > 0 ){
                    $(".cta-disabled")
                        .removeAttr("disabled")
                        .removeClass("cta-disabled");

                    $loadAssessment.next().fadeIn();
                }
            });
        });        

        // var $jsToggleAssessment     = $(".js-toggle-assessment"),
        //     $assessmentList         = $(".assessment-list");

        // $jsToggleAssessment.on("click", function (){
        //     var _this = $(this);
        //     if ( !_this.find(".icon-toggle").hasClass("active") ){
        //         _this
        //             .next()
        //             .slideDown()
        //             .end()
        //             .find(".icon-toggle")
        //             .addClass("active")
        //     } else {
        //         _this
        //             .next()
        //             .slideUp()
        //             .end()
        //             .find(".icon-toggle")
        //             .removeClass("active")
        //     }
        // });
    });

}(jQuery));