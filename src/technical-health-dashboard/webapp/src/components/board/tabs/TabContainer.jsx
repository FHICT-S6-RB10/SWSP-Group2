import React from 'react';
import '../../../styles/board/tabs/tabContainer.css';
import TabButton from "./TabButton";
import MessageIcon from "../MessageIcon";
import {messageTitles, TAB_ICON, UNKNOWN} from "../../../constants";

let tabs = [];

Object.entries(messageTitles).forEach(entry => {
    const level = entry[0];
    const title = `${entry[1]}s`;

    if (level === UNKNOWN) return;
    tabs[level] = {
        title,
        type: level,
        icon: <MessageIcon level={level} usedIn={TAB_ICON}/>
    }
});

tabs.push({
    title: messageTitles[UNKNOWN],
    type: UNKNOWN,
    icon: <MessageIcon level={UNKNOWN} usedIn={TAB_ICON}/>
});

tabs = tabs.filter(tab => tab);

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

    const styledTabs = Object.entries(tabs).map(entry => {
        const key = entry[0]
        const tabInfo = entry[1];
        const {type} = tabInfo;
        return (
                <TabButton
                    key={key}
                    handleClick={() => handleTabClick(type)}
                    isSelected={isSelected(type)}
                    tabInfo={tabInfo}
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
