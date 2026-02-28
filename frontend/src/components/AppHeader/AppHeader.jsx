import React from "react";
import styles from "./AppHeader.module.css";
import logo from "../../assets/resilio-logo.png"; // adjust path if needed

export default function AppHeader({ rightSlot = null }) {
  return (
    <header className={styles.header} role="banner">
      <div className={styles.inner}>
        <div className={styles.brand} aria-label="Resilio home">
  <img
    src={logo}
    alt="Resilio"
    className={styles.logo}
    loading="eager"
  />
</div>

        {rightSlot ? <div className={styles.right}>{rightSlot}</div> : null}
      </div>
    </header>
  );
}