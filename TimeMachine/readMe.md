![image](https://user-images.githubusercontent.com/73463175/99255439-38a92c00-2814-11eb-8415-992489c75cf7.png)

#### General information
This repository of scripts for Revit and other tools is brought to you by the innovation team of Basler&Hofmann (BH).

For more information :
- Our website: https://www.baslerhofmann.ch/
- Our contact person : https://www.baslerhofmann.ch/en/metanavigation/contacts/en-ansprechpartner-detailseite/contact/5902.html

#### Legal information
Time Machine in Revit is project by Basler&Hofmann and was first released in 2020. It's licensed under the MIT license.



## Revit's Time Machine

### Description:
A nice user interface allows you to quickly create Revit filters based of time parameters.

We distinguish 5 cases for filters:
| Case | Colour |
| --- | --- |
| Elements under construction |(_Red_)|
| Elements under demolition |(_Yellow_)| 
| Elements already built |(_Gray_)| 
| Elements not built yet |(_invisible_)|
| Elements already demolished |(_hidden_)| 

### For the plugin to function:
Create 4 project parameters :
- Parameter names:
  - "Construction-start"
  - "Construction-end"
  - "Demolition-start"
  - "Demolition-end"
- Parameter type:
  - Text parameter
  - Values can vary among group instances

Mohamed Nadeem.
