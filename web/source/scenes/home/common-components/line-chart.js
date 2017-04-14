import { Line } from 'react-chartjs-2';
import _ from 'lodash';
import moment from 'moment';
import { formatUtcDate } from 'helpers/dates';

const singleChartDataShape = PropTypes.shape({
  points: PropTypes.arrayOf(
    PropTypes.shape({
      x: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
      y: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    },
  ).isRequired),
  config: PropTypes.shape({
    backgroundColor: PropTypes.string,
    color: PropTypes.string,
  }),
});

export default class DashboardLineChart extends Component {
  static propTypes = {
    charts: PropTypes.oneOfType([
      singleChartDataShape,
      PropTypes.arrayOf(singleChartDataShape),
    ]).isRequired,
    options: PropTypes.shape({
      monthLabels: PropTypes.bool,
      redraw: PropTypes.bool,
    }),
  }

  constructor(props) {
    super(props);

    document.onmousemove = (e) => {
      this.xMousePosition = e.pageX;
      this.yMousePosition = e.pageY;
    };

    this.pointRadius = 4;
    this.xAxesMarksFontSize = this.isMonthLabels() ? 15 : 10;
    this.tintColor = '#5A9DFF';

    const axisTextColor = '#CDCDCD';

    this.options = {
      responsive: true,
      maintainAspectRatio: false,
      hover: {
        intersect: false,
        onHover: this.onChartHover,

      },
      elements: {
        point: {
          radius: 0,
          hoverRadius: this.pointRadius,
        },
      },
      legend: {
        display: false,
      },
      tooltips: {
        mode: 'index',
        backgroundColor: 'rgba(0,0,0,0)',
        intersect: false,
        callbacks: {
          title: this.getTooltipTitle,
          afterTitle: (tooltip) => {
            if (this.isMonthLabels()) {
              return '';
            }
            const date = new Date();
            date.setDate(date.getDate() - (29 - tooltip[0].index));
            return formatUtcDate(date.toISOString());
          },
          label: () => ' ',
        },
        displayColors: false,
        titleMarginBottom: 15,
        position: 'nearest',
        titleFontFamily: 'Open Sans',
        titleFontSize: 15,
        titleFontColor: this.tintColor,
      },
      scales: {
        yAxes: [
          {
            id: 'y-axis-0',
            type: 'linear',
            position: 'left',
            ticks: {
              beginAtZero: true,
              fontFamily: 'Open Sans',
              fontColor: axisTextColor,
            },
            gridLines: {
              color: '#F2F2F2',
              drawBorder: false,
              offsetGridLines: true,
            },
          },
        ],
        xAxes: [
          {
            position: 'bottom',
            ticks: {
              beginAtZero: true,
              fontFamily: 'Open Sans',
              fontColor: axisTextColor,
              fontSize: this.xAxesMarksFontSize,
              maxRotation: 0,
            },
            gridLines: {
              color: 'rgba(0, 0, 0, 0)',
              zeroLineColor: 'rgba(0,0,0,0)',
            },
          },
        ],
      },
      animation: {
        onComplete: this.onCompleteAnimation,
      },
    };
  }

  onChartHover = (event) => {
    const canvas = event.target;
    const position = this.getMousePosition(canvas, event);
    this.isMouseOutOfChart = position.x < 0 || position.y < 0
      || position.x > canvas.clientWidth || position.y > canvas.clientHeight;
    this.canvas = canvas;
  }

  onCompleteAnimation = () => {
    if (this.dashedLine != null && !this.isMouseOutOfChart) {
      this.drawLine(this.canvas, this.dashedLine.x, this.dashedLine.y, this.highlightedXLabelTitle);
    }
  }

  getChartData = () => {
    let xLabels;
    if (this.isMonthLabels()) {
      xLabels = moment.monthsShort();
    } else {
      xLabels = [1];
      for (let i = 2; i <= 30; i += 1) {
        const label = i % 5 === 0 ? i : '•';
        xLabels.push(label);
      }
    }
    return ({
      datasets: this.generateDatasets(),
      xLabels,
    });
  }

  getMousePosition = (canvas, event) => {
    const rect = canvas.getBoundingClientRect();
    return {
      x: event.clientX - rect.left,
      y: event.clientY - rect.top,
    };
  }

  getTooltipTitle = (tooltips) => {
    const x = _.maxBy(tooltips, tooltip => tooltip.x).x;
    const y = _.minBy(tooltips, tooltip => tooltip.y).y;

    let label = '';
    if (tooltips.length > 1) {
      label = [];
      tooltips.forEach((tooltip) => { label = label.concat(tooltip.yLabel); });
    } else {
      label = tooltips[0].yLabel;
    }

    this.dashedLine = { x, y };
    const index = tooltips[0].index;

    this.highlightedXLabelTitle = this.isMonthLabels() ?
      this.getChartData().xLabels[index] :
      index + 1;

    return label;
  }

  generateDatasets = () => {
    const { charts } = this.props;
    const datasets = [];

    const generateDataset = ({ points, backgroundColor, color }) => (
      {
        data: points,
        backgroundColor: backgroundColor || 'rgba(0,0,0,0)',
        borderColor: color || 'rgba(90, 157, 255, 0.45)',
        lineTension: 0,
        pointBackgroundColor: this.tintColor,
        pointBorderColor: this.tintColor,
      }
    );

    if (charts instanceof Array) {
      charts.forEach(
        (chart) => {
          datasets.push(generateDataset({ points: chart.points, ...chart.config }));
        },
      );
    } else {
      datasets.push(generateDataset(charts));
    }

    return datasets;
  }

  isMonthLabels = () => this.props.options && this.props.options.monthLabels;

  drawLine(canvas, x, y, xAxesLabel) {
    const ctx = canvas.getContext('2d');
    const chartYAxesStartPosition = this.isMonthLabels() ? canvas.clientHeight - 32
      : canvas.clientHeight - 25;

    ctx.setLineDash([5, 5]);

    ctx.beginPath();
    ctx.arc(x, chartYAxesStartPosition, this.pointRadius, 0, 2 * Math.PI, false);

    ctx.fillStyle = '#F2F2F2';
    ctx.fill();
    ctx.closePath();

    ctx.beginPath();
    ctx.moveTo(x, chartYAxesStartPosition - this.pointRadius - 3);
    ctx.lineWidth = 2;

    ctx.lineTo(x, y + this.pointRadius);
    ctx.strokeStyle = '#DEEBFE';
    ctx.stroke();

    ctx.setLineDash([0, 0]);
    ctx.closePath();

    const fontSize = this.xAxesMarksFontSize * 1.3;
    ctx.font = `${fontSize}px Open Sans`;
    ctx.textBaseline = 'middle';

    const highlightedText = xAxesLabel;

    const highlightedTextXCoord = Math.round(x - (ctx.measureText(highlightedText).width / 2));
    const highlightedTextYCoord = chartYAxesStartPosition + 17;

    ctx.fillStyle = '#FFF';

    ctx.fillRect(highlightedTextXCoord,
      highlightedTextYCoord - (fontSize / 2),
      ctx.measureText(highlightedText).width,
      fontSize + 5);

    ctx.fillStyle = this.tintColor;

    ctx.fillText(highlightedText, highlightedTextXCoord, highlightedTextYCoord);
  }

  render() {
    const { redraw } = this.props.options || {};
    return (
      <Line
        data={this.getChartData()} options={this.options} redraw={redraw}
      />
    );
  }
}
