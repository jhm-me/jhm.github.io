#!/usr/bin/env python3

"""
Description: This script removes Civil 3D's "i"-labelled faces from a LandXML 1.2 surface.
"""

import lxml.etree as et
import os
import tkinter as tk
from tkinter import filedialog

#code stolen from https://www.knickknackcivil.com/hacking-landxml.html
#and https://forums.autodesk.com/t5/civil-3d-forum/correctly-export-landxml-surface-without-additional/m-p/12425878/highlight/true#M503764

#tk to make dialogs to get input and output paths
root = tk.Tk()
root.withdraw()

#tuple used to define what extensions the file browser will allow us to view
xml_filetype = [
    ("XML Files", "*.xml")
    ]

landxml_in_file = filedialog.askopenfilename(title="Select a LandXML", filetypes=xml_filetype)
landxml_out_file = filedialog.asksaveasfilename(title="Save the Output LandXML", filetypes=xml_filetype, initialdir=os.path.dirname(landxml_in_file), defaultextension=".xml")

with open(landxml_in_file, 'rb') as in_file:
    with open(landxml_out_file, 'wb+') as out_file:
        xml = in_file.read().decode('iso-8859-1').encode('ascii')
        doc = et.XML(xml)
        surfaces = doc.getchildren()[-1]

        # LandXML could contain multiple surfaces
        for surface in surfaces:
            definition = surface.getchildren()[-1]  #Selecting surface definition
            faces = definition.getchildren()[-1]    #Selecting faces

            for face in faces:
                if 'i' in face.attrib:
                    face.getparent().remove(face)

        # Creating output landxml without internal faces and enabling 
        # pretty print which is easier to read and edit.
        out_xml = et.tostring(doc, encoding="UTF-8", pretty_print=True)
        out_file.write(out_xml)
        out_file.close