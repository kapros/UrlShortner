import { allLinksUrl, deleteLinkUrl } from "../utils/Links";
import { reqHeaders } from "../utils/HttpHelpers";
import { useEffect, useState } from "react";
import axios from "axios";

export function AllLinks() {
    const [data, setData] = useState([]);
    
    const deleteLink = async (short) => {
        await axios(deleteLinkUrl + short, {
            method: "DELETE",
            headers: reqHeaders,
        })
        await getAllLinks();
    }

    const getAllLinks = async () => {
        const res = await axios(allLinksUrl, {
            method: "GET",
            headers: reqHeaders,
        })
        setData(res.data.shortUrls);
    }
    
    useEffect(() => {
        getAllLinks()
        .catch(console.log);
    }, [])


    return (  
        <>        
        <div className="links">
        { 
            data.length > 0 ? 
                data.map((link) => (
                <div className="container-links" key={link.code}>
                    <div>
                        <a href={link.longLink} target="_blank">{ link.longLink }</a> 
                    </div>
                    <div>Short link:</div>
                    <div className="short-link">
                        <a href={link.shortLink} target="_blank">{ link.shortLink }</a> 
                        <button className="btn" onClick={() => deleteLink(link.code)}>Delete</button> 
                    </div>
                </div> 
                ))
            :
                <div>
                    No links yet!
                </div>
        }
        </div>
        </>
    )
}