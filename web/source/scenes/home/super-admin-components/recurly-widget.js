import styles from './recurly-widget.module.css';

const description = 'Monotonectally orchestrate standardized ROI without intuitive  e-services. Synergisitcally target market positioning products and transparent process improvements.';

const RecurlyWidget = () => (
  <div className={styles.rootContainer}>
    <div className={styles.recurlyLogo} onClick={() => window.open('https://recurly.com/')} />
    <div className={styles.description}>
      {description}
    </div>
  </div>
);

export default RecurlyWidget;
