$(function(){
	
	/* Morris Area Chart */
	
	window.mA = Morris.Area({
	    element: 'morrisArea',
	    data: [
	        { y: '2013', a: 60},
	        { y: '2014', a: 100},
	        { y: '2015', a: 240},
	        { y: '2016', a: 120},
	        { y: '2017', a: 80},
	        { y: '2018', a: 100},
	        { y: '2019', a: 300},
	    ],
	    xkey: 'y',
	    ykeys: ['a'],
	    labels: ['Revenue'],
	    lineColors: ['#1b5a90'],
	    lineWidth: 2,
		
     	fillOpacity: 0.5,
	    gridTextSize: 10,
	    hideHover: 'auto',
	    resize: true,
		redraw: true
	});
	
	/* Morris Line Chart */
	
	window.mL = Morris.Line({
	    element: 'morrisLine',
	    data: [
	        { y: '2015', a: 100, b: 30},
	        { y: '2016', a: 20,  b: 60},
	        { y: '2017', a: 90,  b: 120},
	        { y: '2018', a: 50,  b: 80},
	        { y: '2019', a: 120,  b: 150},
	    ],
	    xkey: 'y',
	    ykeys: ['a', 'b'],
	    labels: ['Mentors', 'Mentees'],
	    lineColors: ['#1b5a90','#ff9d00'],
	    lineWidth: 1,
	    gridTextSize: 10,
	    hideHover: 'auto',
	    resize: true,
		redraw: true
	});

	window.mD = Morris.Donut({
		element: 'morrisDonut',
		data: [
			{ label: "Tickets Sales", value: 60 },
			{ label: "Merchandise Sales", value: 30 },
			{ label: "Other Revenue", value: 10 }
		],
		colors: ['#1b5a90', '#ff9d00', '#0c9'],
		resize: true
	});
	$(window).on("resize", function(){
		mA.redraw();
		mL.redraw();
		mD.redraw();
	});

});