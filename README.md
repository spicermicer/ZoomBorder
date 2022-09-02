# ZoomBorder
Provides a zoom border that allows internal elements to be zoomed and panned. 

It's a very basic single control library, the original code was stolen from the following stackoverflow answer [Here](https://stackoverflow.com/a/6782715)


#Install
Install via nuget using the following cmd line
` Install-Package spicermicer.ZoomBorder -Version 1.0.0 `

# Example Usage

## Basic Usage

First import the library to the userspace

>   xmlns:zoomable="clr-namespace:ZoomBorder;assembly=ZoomBorder"


Then implement the control with whatever control you want inside
> 
    <zoomable:ZoomBorder>
        <Image Source="..."/>
    </zoomable:ZoomBorder>


## Defining actions

By default the zoompanel with use left click to pan, and right click to reset the zoom function. These can be modified by using the following code
>
    <zoomable:ZoomBorder            
            LeftButtonAction="None"
            RightButtonAction="Move"
            MiddleButtonAction="Reset">    
        <Image Source="..."/>
    </zoomable:ZoomBorder>

## Zoom range

You can modify the limits on the zoom by using the following
> 
    <zoomable:ZoomBorder
        ScaleMin="0.5"
        ScaleMax="10">
        <Image Source="..."/>
    </zoomable:ZoomBorder>

This will provide a zoom panel where the user can zoom out so the contents is half the original size (0.5) and zoom in to where the contents in 10 times the original size

# Example
An example of the zoompanels usage can be found in [Example](Example)
