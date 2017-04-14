import { Line } from 'rc-progress';

const ProgressBar = ({ percent, strokeColor }) => {
  let color = '#333';

  if (percent >= 100) {
    color = '#5A9DFF';
  }

  return (
    <Line
      percent={percent > 100 ? 100 : percent}
      strokeWidth='1.7'
      strokeColor={strokeColor || color}
      trailWidth='1.6'
    />
  );
};
export default ProgressBar;

ProgressBar.propTypes = {
  percent: PropTypes.number.isRequired,
  strokeColor: PropTypes.string,
};
