import { Chart, Doughnut } from 'react-chartjs-2';

export default class DashboardDoughnutChart extends Component {
  static propTypes = {
    data: PropTypes.arrayOf(
      PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    ).isRequired,
    options: PropTypes.shape({
      redraw: PropTypes.bool,
    }),
  }

  constructor(props) {
    super(props);
    this.drawStatisticsTextIntoDoughnut = this.drawStatisticsTextIntoDoughnut.bind(this);
    this.options = {
      responsive: true,
      maintainAspectRatio: false,
      cutoutPercentage: 80,
      tooltips: {
        enabled: false,
      },
    };

    this.data = {
      datasets: [{
        backgroundColor: [
          '#5A9DFF',
          '#333333',
        ],
        hoverBackgroundColor: [
          '#5A9DFF',
          '#333333',
        ],
        borderColor: '#FFF',
        hoverBorderColor: '#FFF',
        borderWidth: 2,
        data: this.props.data,
      }],
    };
  }

  componentWillMount() {
    Chart.plugins.register({
      afterDraw: this.drawStatisticsTextIntoDoughnut,
    });
  }

  drawStatisticsTextIntoDoughnut(object) {
    if (object.chart.config.type === 'doughnut') {
      this.chart = object.chart;

      const width = this.chart.width;
      const height = this.chart.height;

      let fontSize = 16;
      this.chart.ctx.font = `${fontSize}px Open Sans`;
      this.chart.ctx.textBaseline = 'middle';
      const generalHeight = height / 2;

      const firstStatText = `${this.props.data[1]}%`;
      const firstStatX = Math.round(
        (width / 2) - this.chart.ctx.measureText(firstStatText).width - 10,
      );

      this.chart.ctx.fillStyle = '#333';

      const secondStatText = `${this.props.data[0]}%`;
      const secondStatX = Math.round((width / 2) + 10);
      this.chart.ctx.fillText(firstStatText, firstStatX, generalHeight);
      this.chart.ctx.fillText(secondStatText, secondStatX, generalHeight);

      fontSize = 25;
      this.chart.ctx.font = `${fontSize}px Open Sans`;

      const gapText = '|';
      const gapX = Math.round((width / 2) - (this.chart.ctx.measureText(gapText).width / 2));

      this.chart.ctx.fillStyle = '#989898';
      this.chart.ctx.fillText(gapText, gapX, generalHeight - 3);
    }
  }

  render() {
    const { redraw } = this.props.options || {};
    return (
      <Doughnut data={this.data} options={this.options} redraw={redraw} />
    );
  }
}
