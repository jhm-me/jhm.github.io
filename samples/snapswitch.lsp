;; snapswitch.lsp
;; toggle osnaps between a default preferred option and a specific case. could be more efficient, but whatever..
;; by: jhm
;; originally created: 2023-10-03
;; last updated: 2024-07-24
(defun snapswitch (new_osmode / default_osmode current_osmode) ;; new_osmode is passed to from calling the function. default/current are local vars
  (setq default_osmode 5199) ;; magic number, based on my personal preferences.
  (setq current_osmode (getvar "osmode"))
  
  (if (= current_osmode default_osmode)
    (
      cond
      ((= new_osmode "non") (setvar "osmode" 0))
      ((= new_osmode "end") (setvar "osmode" 1))
      ((= new_osmode "mid") (setvar "osmode" 2))
      ((= new_osmode "cen") (setvar "osmode" 4))
      ((= new_osmode "nod") (setvar "osmode" 8))
      ((= new_osmode "per") (setvar "osmode" 128))
      ((= new_osmode "nea") (setvar "osmode" 512))
      ((= new_osmode "gce") (setvar "osmode" 1024))
      ((= new_osmode "app") (setvar "osmode" 2048))
      (t (princ "new_osmode not defined - osmode not changed."))
    )
  (setvar "osmode" default_osmode) ;;else case when you're not already in the default os_mode
  )
  (princ)
)