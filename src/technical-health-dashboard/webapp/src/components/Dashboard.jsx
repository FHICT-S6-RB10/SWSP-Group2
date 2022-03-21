import React from 'react';
import '../styles/dashboard.css';
import ServiceList from "./ServiceList";

const Dashboard = () => {
    return (
        <div className="dashboard">
            <ServiceList/>
        </div>
    )
}

export default Dashboard
