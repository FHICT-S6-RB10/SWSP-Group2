import React from 'react';
import '../styles/dashboard.css';
import ServiceContainer from "./services/ServiceContainer";
import Board from "./board/Board";

const Dashboard = () => {
    return (
        <div className="dashboard">
            <ServiceContainer/>
            <Board/>
        </div>
    )
}

export default Dashboard
