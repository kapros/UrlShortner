import { newLinkUrl } from "../utils/Links";
import { reqHeaders } from "../utils/HttpHelpers";
import { useForm } from "react-hook-form";
import { useState } from "react";
import axios from "axios";

function NewLink() {
    const { register, handleSubmit } = useForm();
    const [data, setData] = useState(null);
    
    const createLink = async (data) => {
        const body = { url: data.url };
        const res = await axios(newLinkUrl, {
            method: "POST",
            data: body,
            headers: reqHeaders,
        })
        setData(res.data);
    }
    
    const copyToClipboard = () => {
        navigator.clipboard.writeText(data.shortUrl);
    }

    return (  
        <>
            <div className="container">
                <div>Insert a link to get a shorter version:
                <form onSubmit={handleSubmit(createLink)}>
                    <input data-test-id="links-input" type="url" name="url" {...register("url")} placeholder="https://www.youtube.com/watch?v=dQw4w9WgXcQ" />
                    <button type="submit">Shorten!</button>
                </form>
            </div>
            
            { 
                data && 
                <div>
                    <span>Short link:</span>
                    <div>
                    <a href={data.shortUrl} target="_blank" data-test-id="short-link" >{ data.shortUrl }</a> 
                    </div>
                    <div> 
                        <button className="btn" onClick={copyToClipboard}>Copy to clipboard</button> 
                    </div>
                </div> 
            }
            </div>
        </>
    )
}

export default NewLink;
