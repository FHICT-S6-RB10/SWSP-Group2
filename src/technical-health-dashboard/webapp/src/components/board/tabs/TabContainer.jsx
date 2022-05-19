import React from 'react';
import '../../../styles/board/tabs/tabContainer.css';
import TabButton from "./TabButton";
import MessageIcon from "../MessageIcon";
import {messageTitles, TAB_ICON} from "../../../constants";

const tabs = Object.entries(messageTitles).map(entry => {
    const level = entry[0];
    const title = `${entry[1]}s`;

    return {
        title,
        type: level,
        icon: <MessageIcon level={level} usedIn={TAB_ICON}/>
    }
});

const TabContainer = props => {
    const {selectedTabs, setSelectedTabs} = props;

    const isSelected = id => {
        return selectedTabs.indexOf(id) > -1;
    }

    const handleTabClick = clickedTabId => {
        if(isSelected(clickedTabId)) {
            setSelectedTabs(selectedTabs.filter(tabId => tabId !== clickedTabId));
        } else {
            setSelectedTabs([...selectedTabs, clickedTabId]);
        }
    }

    const styledTabs = tabs.map(tab => {
        const {type} = tab;
        return (
                <TabButton
                    key={type}
                    handleClick={() => handleTabClick(type)}
                    isSelected={isSelected(type)}
                    tabInfo={tab}
                />
            )
        }
    );

    return (
        <div className="tab-container">
            {styledTabs}
        </div>
    )
}

export default TabContainer
