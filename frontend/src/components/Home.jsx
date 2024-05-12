import { Link, Outlet } from "react-router-dom";

export function Home() {
    return <>
        <div className="navbar">
            <button><Link to="new">New</Link></button>
            <button><Link to="all-links">All links</Link></button>
        </div>
        <Outlet />
        </>

        
}