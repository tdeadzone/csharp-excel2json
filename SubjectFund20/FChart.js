var chart = document.getElementById("chart");

var db = window.__data__;
var timeItems = db.timeItems;
var fundItems = db.fundItems;

var fetch = function (key, type) {
	if (!key) throw new Error("key404");
	var result = [];
	Object.keys(timeItems).forEach(function (time) {
		result.push(timeItems[time][key][type]);
	});
	return result;
};

var select = function (key, type) {
	return db[key][type];
};

/**
 * 生成对应图形的option
 * @param {any} chart 具体4个图的那一个
 * @param {any} opts  具体的这个图的配置（如：是否剔除货基开关all rid，不同的基金种类开关 diff id）
 */
optsGen = function optsGen(chartType, type, id) {
	// TODO非法检测
	//console.log("chartType->", chartType, "type->", type, "id->", id);
	// 动态数据
	

	if (chartType === "c1") {
		if (!id) {
			var timeKey = Object.keys(timeItems);

			var maxLeftY = select("timeItems", "2017")[type].fundWorth;
			maxLeftY = Math.ceil(maxLeftY / 10000 + 1) * 10000 * 1.1;

			var maxRightY = select("timeItems", "2017")[type].fundAmount;
			maxRightY *= 1.1;

			var worthData = fetch(type, "fundWorth");
			var amountData = fetch(type, "fundAmount")
		} else {
			var timeKey = ['1998', '1999', '2000', '2001', '2002', '2003', '2004', '2005', '2006', '2007', '2008', '2009', '2010', '2011', '2012', '2013', '2014', '2015', '2016', '2017'];

			var worthData = [];
			var amountData = [];
			fundItems[id].data[type].forEach(function (item) {
				amountData.push(item.fundAmount);
				worthData.push(item.fundWorth);
			});
		}

		return {
			tooltip: {
				trigger: "axis",
				axisPointer: {
					type: "cross",
					crossStyle: {
						color: "#999"
					}
				}
			},
			legend: {
				data: ["资产净值", "基金数量"]
			},
			xAxis: [
				{
					type: "category",
					data: timeKey
				}
			],
			yAxis: [
				{
					type: "value",
					name: "资产净值（亿元）",
					min: 0,
					max: maxLeftY,
					interval: Math.floor(maxLeftY / 5),
					axisLabel: { formatter: "{value}" }
				},
				{
					type: "value",
					name: "基金数量（只）",
					min: 0,
					max: maxRightY,
					interval: Math.floor(maxRightY / 5),
					axisLabel: { formatter: "{value}" }
				}
			],
			series: [
				{
					name: "资产净值",
					type: "line",
					data: worthData
				},
				{
					name: "基金数量",
					type: "line",
					yAxisIndex: 1,
					data: amountData
				}
			]
		};
	}

	if (chartType === "c2") {
		return {
			color: ['#3398DB'],
			tooltip: {
				axisPointer: {
					crossStyle: {
						color: "#999"
					}
				}
			},
			xAxis: [
				{
					type: 'category',
					data: timeKey,
					axisTick: {
						alignWithLabel: true
					}
				}
			],
			yAxis: [
				{
					type: 'value',
					name: "基金公司家数（个）",
				}
			],
			series: [
				{
					name: '基金公司家数：',
					type: 'bar',
					barWidth: '30%',
					data: fetch(type, "fundCoNumber")
				}
			]
		}
	}

};

var opt = optsGen("c1", "all");

echart = echarts.init(chart);
echart.setOption(opt);
