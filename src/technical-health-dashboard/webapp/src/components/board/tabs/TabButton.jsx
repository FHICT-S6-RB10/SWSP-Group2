import React from 'react';
import '../../../styles/board/tabs/tabButton.css';
import ReactTooltip from "react-tooltip";

const TabButton = props => {
    const {icon, title} = props.tabInfo;
    const {handleClick, isSelected} = props;
    return (
        <div
            className={`tab-button ${isSelected && "tab-button-selected"} ${title && title.length < 7 && "short"}`}
            onClick={handleClick} data-tip={`${isSelected ? 'Hide' : 'Show'} ${title}`}
        >
            <div className='tab-icon-wrapper'>
                {icon}
            </div>
            <div className='tab-title-wrapper'>
                <span className="tab-title">
                    {title}
                </span>
            </div>
            <ReactTooltip/>
        </div>
    )
}

export default TabButton
