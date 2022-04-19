import React from 'react';
import '../../../styles/board/tabs/tabContainer.css';
import TabButton from "./TabButton";
import MessageIcon from "../MessageIcon";
import {ERROR, LOG, UNKNOWN, WARNING} from "../../../constants";

const tabs = {
    [LOG]: {
        title: "Logs",
        icon: <MessageIcon level={LOG}/>
    },
    [WARNING]: {
        title: "Warnings",
        icon: <MessageIcon level={WARNING}/>
    },
    [ERROR]: {
        title: "Errors",
        icon: <MessageIcon level={ERROR}/>
    },
    [UNKNOWN]: {
        title: "Unknown",
        icon: <MessageIcon level={UNKNOWN}/>
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
