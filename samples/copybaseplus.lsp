;; CopyBasePlus.lsp
;; switches to WCS if not applicable, copybases from 0,0,0, switches back to previous UCS if applicable
;; by: jhm
;; originally created: 10-02-2023
;; last updated: 10-02-2023
(defun c:cpbp ()
  (if (ssget "_p") ;; if previous selection set exists (aka isn't nil)
      (if (/= (getvar "ucsname") "") ;;ucsname = "" means you're in world.
        (progn
          (command "._ucs" "w")
          (command "._copybase" "0,0,0" "p" "")
          (command "._ucs" "p")
          (princ)
        )
        (progn
          (command "._copybase" "0,0,0" "p" "")
          (princ)
        )
      )
    (prompt "no previous selection set. please select objects and retry.") ;;case of previous ss = nil
  )
  (princ)
)