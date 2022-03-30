import React from 'react';
import '../../../styles/board/tabs/tabButton.css';

const TabButton = (props) => {
    const {icon, title} = props.tabInfo;
    const {handleClick, isSelected} = props;
    return (
        <div
            className={`tab-button ${isSelected && "tab-button-selected"}`}
            onClick={handleClick}
        >
            {icon}
            <span className="tab-button-title">{title}</span>
        </div>
    )
}

export default TabButton
