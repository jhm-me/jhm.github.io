;; assign coordinate systems based on the airport identifier
;; jhm
;; last updated: 2024-08-07

;; beginning of this code is from
;; https://www.theswamp.org/index.php?topic=56410.0

(vl-load-com)
(defun c:acs ( / airport C3D C3DDOC cs cssettings file line *error* )
  
  (defun *error* ( msg )
    (if (not (member msg '("Function cancelled" "quit / exit abort")))
        (princ (strcat "\nError: " msg))
    )
    (princ)
  )
  
  (setq 
    C3D (strcat "HKEY_LOCAL_MACHINE\\" (if vlax-user-product-key (vlax-user-product-key) (vlax-product-key)))
    ;; e.g. "HKEY_LOCAL_MACHINE\\Software\\Autodesk\\AutoCAD\\R24.3\\ACAD-7100:409"
    C3D (vl-registry-read C3D "Release")
    ;; e.g. "13.6.342.0"
    C3D (substr C3D 1 (vl-string-search "." C3D (+ (vl-string-search "." C3D) 1)))
    ;; e.g. "13.6"
    C3D (vla-getinterfaceobject (vlax-get-acad-object) (strcat "AeccXUiLand.AeccApplication." C3D))
  )
  (setq C3Ddoc (vla-get-activedocument C3D))
  (setq cssettings (vlax-get (vlax-get (vlax-get (vlax-get c3ddoc 'settings) 'drawingsettings) 'unitzonesettings) 'coordinatesystem))

  (setq airport (strcase (getstring "Enter Airport: ") T))
  
  (setq file (open "C:\\INPUT YOUR PATH HERE\\airport_list.txt" "r")) ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;; change this!!! :) :)

  ;; input (txt file) example formatting:
    ;; 1f4,OK83-SF
    ;; okv,VA83-NF
    ;; ord,IL83-EF
    ;; orf,VA83-SF
  ;; etc etc ...
  
  (setq line (read-line file))
    
  (while (/= line nil)
    (setq id (strcase (substr line 1 (vl-string-search "," line)) T))
    (if (= airport id) ;; if we find our airport, stop early.
      (progn 
        (setq cs (strcase (substr line (+ 2 (vl-string-search "," line)))))
        (setq line nil)
      )
        (setq line (read-line file))
    )
  )
  (close file)


  (if cs 
    (progn 
      (vlax-put cssettings 'cscode cs)
      (princ (strcat "Coordinate System Applied: " cs));; if it exists
    )
    (alert "That airport was not in the list.") ;; if nil
  )
  (princ)
)