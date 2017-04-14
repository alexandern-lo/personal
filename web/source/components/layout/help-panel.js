import { SkyLightStateless } from 'react-skylight';

const styles = {
  borderRadius: '0px',
  height: '100%',
  width: '50%',
  top: '50px',
  left: '100%',
  marginTop: '0px',
  marginLeft: '-52%',
};

const HelpPanel = ({ isOpened, onClose }) => (
  <SkyLightStateless
    dialogStyles={styles}
    onOverlayClicked={onClose}
    onCloseClicked={onClose}
    isVisible={isOpened}
    title='Help'
  />
);
export default HelpPanel;

HelpPanel.propTypes = {
  isOpened: PropTypes.bool,
  onClose: PropTypes.func.isRequired,
};
