import React from 'react';
import '../../../styles/board/tabs/tabContainer.css';
import TabButton from "./TabButton";
import MessageIcon from "../MessageIcon";
import {ERROR, LOG, TAB_ICON, UNKNOWN, WARNING} from "../../../constants";

const tabs = {
    [UNKNOWN]: {
        title: "Unknown",
        icon: <MessageIcon level={UNKNOWN} usedIn={TAB_ICON}/>
    },
    [LOG]: {
        title: "Logs",
        icon: <MessageIcon level={LOG} usedIn={TAB_ICON}/>
    },
    [WARNING]: {
        title: "Warnings",
        icon: <MessageIcon level={WARNING} usedIn={TAB_ICON}/>
    },
    [ERROR]: {
        title: "Errors",
        icon: <MessageIcon level={ERROR} usedIn={TAB_ICON}/>
    }
}

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

    const styledTabs = Object.keys(tabs).map(key => (
            <TabButton
                key={key}
                handleClick={() => handleTabClick(key)}
                isSelected={isSelected(key)}
                tabInfo={tabs[key]}
            />
        )
    );

    return (
        <div className="tab-container">
            {styledTabs}
        </div>
    )
}

export default TabContainer
