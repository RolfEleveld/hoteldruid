<?php exit(); ?>

<!--             2025-11-23 21:21:10             -->

<!--  **    SAVE THIS FILE AS hoteld_backup.php     **  -->

<!--  **  SALVA QUESTO FILE COME hoteld_backup.php  **  -->


<backup>
<versione>3.07</versione>
<log>NO</log>
<file>
<nomefile>./dati/lingua.php</nomefile>
<contenuto>
<?php
$lingua[1] = "en";
$lingua[2] = "en";
$lingua[3] = "en";
?></contenuto>
</file>
<file>
<nomefile>./dati/unit.php</nomefile>
<contenuto>
<?php
$unit['s_n'] = $trad_var['room'];
$unit['p_n'] = $trad_var['rooms'];
$unit['gender'] = $trad_var['room_gender'];
$unit['special'] = 0;
$car_spec = explode(",",$trad_var['special_characters']);
for ($num1 = 0 ; $num1 < count($car_spec) ; $num1++) if (substr($unit['p_n'],0,strlen($car_spec[$num1])) == $car_spec[$num1]) $unit['special'] = 1;
?></contenuto>
</file>
<file>
<nomefile>./dati/unit_single.php</nomefile>
<contenuto>
<?php
$unit['s_n'] = $trad_var['bed'];
$unit['p_n'] = $trad_var['beds'];
$unit['gender'] = $trad_var['bed_gender'];
$unit['special'] = 0;
$car_spec = explode(",",$trad_var['special_characters']);
for ($num1 = 0 ; $num1 < count($car_spec) ; $num1++) if (substr($unit['p_n'],0,strlen($car_spec[$num1])) == $car_spec[$num1]) $unit['special'] = 1;
?></contenuto>
</file>
<file>
<nomefile>./dati/tema.php</nomefile>
<contenuto>
<?php
$parole_sost = 0;
$tema[1] = "blu";
$tema[2] = "blu";
$tema[3] = "blu";
?></contenuto>
</file>
<file>
<nomefile>./dati/selectappartamenti.php</nomefile>
<contenuto>
<?php 
echo '
<option value="15">15</option>
<option value="16">16</option>
<option value="17">17</option>
<option value="18">18</option>
<option value="19">19</option>
<option value="20">20</option>
<option value="201">201</option>
<option value="202">202</option>
<option value="203">203</option>
<option value="204">204</option>
<option value="205">205</option>
<option value="206">206</option>
<option value="207">207</option>
<option value="208">208</option>
<option value="209">209</option>
<option value="21">21</option>
<option value="210">210</option>
<option value="212">212</option>
<option value="213">213</option>
<option value="214">214</option>
<option value="215">215</option>
';
?></contenuto>
</file>
<file>
<nomefile>./dati/versione.php</nomefile>
<contenuto>
<?php
define('C_VERSIONE_ATTUALE',3.07);
define('C_DIFF_ORE',0);
define('C_MIN_SESSIONE',90);
define('C_USA_COOKIES',0);
?></contenuto>
</file>
<file>
<nomefile>./dati/abilita_login</nomefile>
<contenuto>
</contenuto>
</file>
<file>
<nomefile>./dati/selectperiodi2025.1.php</nomefile>
<contenuto>
<?php 

$y_ini_menu = array();
$m_ini_menu = array();
$d_ini_menu = array();
$n_dates_menu = array();
$d_increment = array();
$y_ini_menu[0] = "2025";
$m_ini_menu[0] = "0";
$d_ini_menu[0] = "01";
$n_dates_menu[0] = "731";
$d_increment[0] = "1";
$d_names = "\" Su\",\" Mo\",\" Tu\",\" We\",\" Th\",\" Fr\",\" Sa\"";
$m_names = "\"Jan\",\"Feb\",\"Mar\",\"Apr\",\"May\",\"Jun\",\"Jul\",\"Aug\",\"Sep\",\"Oct\",\"Nov\",\"Dec\"";

$dates_options_list = "

<option value=\"2025-01-01\">Jan 01 We, 2025</option>
<option value=\"2025-01-02\">Jan 02 Th, 2025</option>
<option value=\"2025-01-03\">Jan 03 Fr, 2025</option>
<option value=\"2025-01-04\">Jan 04 Sa, 2025</option>
<option value=\"2025-01-05\">Jan 05 Su, 2025</option>
<option value=\"2025-01-06\">Jan 06 Mo, 2025</option>
<option value=\"2025-01-07\">Jan 07 Tu, 2025</option>
<option value=\"2025-01-08\">Jan 08 We, 2025</option>
<option value=\"2025-01-09\">Jan 09 Th, 2025</option>
<option value=\"2025-01-10\">Jan 10 Fr, 2025</option>
<option value=\"2025-01-11\">Jan 11 Sa, 2025</option>
<option value=\"2025-01-12\">Jan 12 Su, 2025</option>
<option value=\"2025-01-13\">Jan 13 Mo, 2025</option>
<option value=\"2025-01-14\">Jan 14 Tu, 2025</option>
<option value=\"2025-01-15\">Jan 15 We, 2025</option>
<option value=\"2025-01-16\">Jan 16 Th, 2025</option>
<option value=\"2025-01-17\">Jan 17 Fr, 2025</option>
<option value=\"2025-01-18\">Jan 18 Sa, 2025</option>
<option value=\"2025-01-19\">Jan 19 Su, 2025</option>
<option value=\"2025-01-20\">Jan 20 Mo, 2025</option>
<option value=\"2025-01-21\">Jan 21 Tu, 2025</option>
<option value=\"2025-01-22\">Jan 22 We, 2025</option>
<option value=\"2025-01-23\">Jan 23 Th, 2025</option>
<option value=\"2025-01-24\">Jan 24 Fr, 2025</option>
<option value=\"2025-01-25\">Jan 25 Sa, 2025</option>
<option value=\"2025-01-26\">Jan 26 Su, 2025</option>
<option value=\"2025-01-27\">Jan 27 Mo, 2025</option>
<option value=\"2025-01-28\">Jan 28 Tu, 2025</option>
<option value=\"2025-01-29\">Jan 29 We, 2025</option>
<option value=\"2025-01-30\">Jan 30 Th, 2025</option>
<option value=\"2025-01-31\">Jan 31 Fr, 2025</option>
<option value=\"2025-02-01\">Feb 01 Sa, 2025</option>
<option value=\"2025-02-02\">Feb 02 Su, 2025</option>
<option value=\"2025-02-03\">Feb 03 Mo, 2025</option>
<option value=\"2025-02-04\">Feb 04 Tu, 2025</option>
<option value=\"2025-02-05\">Feb 05 We, 2025</option>
<option value=\"2025-02-06\">Feb 06 Th, 2025</option>
<option value=\"2025-02-07\">Feb 07 Fr, 2025</option>
<option value=\"2025-02-08\">Feb 08 Sa, 2025</option>
<option value=\"2025-02-09\">Feb 09 Su, 2025</option>
<option value=\"2025-02-10\">Feb 10 Mo, 2025</option>
<option value=\"2025-02-11\">Feb 11 Tu, 2025</option>
<option value=\"2025-02-12\">Feb 12 We, 2025</option>
<option value=\"2025-02-13\">Feb 13 Th, 2025</option>
<option value=\"2025-02-14\">Feb 14 Fr, 2025</option>
<option value=\"2025-02-15\">Feb 15 Sa, 2025</option>
<option value=\"2025-02-16\">Feb 16 Su, 2025</option>
<option value=\"2025-02-17\">Feb 17 Mo, 2025</option>
<option value=\"2025-02-18\">Feb 18 Tu, 2025</option>
<option value=\"2025-02-19\">Feb 19 We, 2025</option>
<option value=\"2025-02-20\">Feb 20 Th, 2025</option>
<option value=\"2025-02-21\">Feb 21 Fr, 2025</option>
<option value=\"2025-02-22\">Feb 22 Sa, 2025</option>
<option value=\"2025-02-23\">Feb 23 Su, 2025</option>
<option value=\"2025-02-24\">Feb 24 Mo, 2025</option>
<option value=\"2025-02-25\">Feb 25 Tu, 2025</option>
<option value=\"2025-02-26\">Feb 26 We, 2025</option>
<option value=\"2025-02-27\">Feb 27 Th, 2025</option>
<option value=\"2025-02-28\">Feb 28 Fr, 2025</option>
<option value=\"2025-03-01\">Mar 01 Sa, 2025</option>
<option value=\"2025-03-02\">Mar 02 Su, 2025</option>
<option value=\"2025-03-03\">Mar 03 Mo, 2025</option>
<option value=\"2025-03-04\">Mar 04 Tu, 2025</option>
<option value=\"2025-03-05\">Mar 05 We, 2025</option>
<option value=\"2025-03-06\">Mar 06 Th, 2025</option>
<option value=\"2025-03-07\">Mar 07 Fr, 2025</option>
<option value=\"2025-03-08\">Mar 08 Sa, 2025</option>
<option value=\"2025-03-09\">Mar 09 Su, 2025</option>
<option value=\"2025-03-10\">Mar 10 Mo, 2025</option>
<option value=\"2025-03-11\">Mar 11 Tu, 2025</option>
<option value=\"2025-03-12\">Mar 12 We, 2025</option>
<option value=\"2025-03-13\">Mar 13 Th, 2025</option>
<option value=\"2025-03-14\">Mar 14 Fr, 2025</option>
<option value=\"2025-03-15\">Mar 15 Sa, 2025</option>
<option value=\"2025-03-16\">Mar 16 Su, 2025</option>
<option value=\"2025-03-17\">Mar 17 Mo, 2025</option>
<option value=\"2025-03-18\">Mar 18 Tu, 2025</option>
<option value=\"2025-03-19\">Mar 19 We, 2025</option>
<option value=\"2025-03-20\">Mar 20 Th, 2025</option>
<option value=\"2025-03-21\">Mar 21 Fr, 2025</option>
<option value=\"2025-03-22\">Mar 22 Sa, 2025</option>
<option value=\"2025-03-23\">Mar 23 Su, 2025</option>
<option value=\"2025-03-24\">Mar 24 Mo, 2025</option>
<option value=\"2025-03-25\">Mar 25 Tu, 2025</option>
<option value=\"2025-03-26\">Mar 26 We, 2025</option>
<option value=\"2025-03-27\">Mar 27 Th, 2025</option>
<option value=\"2025-03-28\">Mar 28 Fr, 2025</option>
<option value=\"2025-03-29\">Mar 29 Sa, 2025</option>
<option value=\"2025-03-30\">Mar 30 Su, 2025</option>
<option value=\"2025-03-31\">Mar 31 Mo, 2025</option>
<option value=\"2025-04-01\">Apr 01 Tu, 2025</option>
<option value=\"2025-04-02\">Apr 02 We, 2025</option>
<option value=\"2025-04-03\">Apr 03 Th, 2025</option>
<option value=\"2025-04-04\">Apr 04 Fr, 2025</option>
<option value=\"2025-04-05\">Apr 05 Sa, 2025</option>
<option value=\"2025-04-06\">Apr 06 Su, 2025</option>
<option value=\"2025-04-07\">Apr 07 Mo, 2025</option>
<option value=\"2025-04-08\">Apr 08 Tu, 2025</option>
<option value=\"2025-04-09\">Apr 09 We, 2025</option>
<option value=\"2025-04-10\">Apr 10 Th, 2025</option>
<option value=\"2025-04-11\">Apr 11 Fr, 2025</option>
<option value=\"2025-04-12\">Apr 12 Sa, 2025</option>
<option value=\"2025-04-13\">Apr 13 Su, 2025</option>
<option value=\"2025-04-14\">Apr 14 Mo, 2025</option>
<option value=\"2025-04-15\">Apr 15 Tu, 2025</option>
<option value=\"2025-04-16\">Apr 16 We, 2025</option>
<option value=\"2025-04-17\">Apr 17 Th, 2025</option>
<option value=\"2025-04-18\">Apr 18 Fr, 2025</option>
<option value=\"2025-04-19\">Apr 19 Sa, 2025</option>
<option value=\"2025-04-20\">Apr 20 Su, 2025</option>
<option value=\"2025-04-21\">Apr 21 Mo, 2025</option>
<option value=\"2025-04-22\">Apr 22 Tu, 2025</option>
<option value=\"2025-04-23\">Apr 23 We, 2025</option>
<option value=\"2025-04-24\">Apr 24 Th, 2025</option>
<option value=\"2025-04-25\">Apr 25 Fr, 2025</option>
<option value=\"2025-04-26\">Apr 26 Sa, 2025</option>
<option value=\"2025-04-27\">Apr 27 Su, 2025</option>
<option value=\"2025-04-28\">Apr 28 Mo, 2025</option>
<option value=\"2025-04-29\">Apr 29 Tu, 2025</option>
<option value=\"2025-04-30\">Apr 30 We, 2025</option>
<option value=\"2025-05-01\">May 01 Th, 2025</option>
<option value=\"2025-05-02\">May 02 Fr, 2025</option>
<option value=\"2025-05-03\">May 03 Sa, 2025</option>
<option value=\"2025-05-04\">May 04 Su, 2025</option>
<option value=\"2025-05-05\">May 05 Mo, 2025</option>
<option value=\"2025-05-06\">May 06 Tu, 2025</option>
<option value=\"2025-05-07\">May 07 We, 2025</option>
<option value=\"2025-05-08\">May 08 Th, 2025</option>
<option value=\"2025-05-09\">May 09 Fr, 2025</option>
<option value=\"2025-05-10\">May 10 Sa, 2025</option>
<option value=\"2025-05-11\">May 11 Su, 2025</option>
<option value=\"2025-05-12\">May 12 Mo, 2025</option>
<option value=\"2025-05-13\">May 13 Tu, 2025</option>
<option value=\"2025-05-14\">May 14 We, 2025</option>
<option value=\"2025-05-15\">May 15 Th, 2025</option>
<option value=\"2025-05-16\">May 16 Fr, 2025</option>
<option value=\"2025-05-17\">May 17 Sa, 2025</option>
<option value=\"2025-05-18\">May 18 Su, 2025</option>
<option value=\"2025-05-19\">May 19 Mo, 2025</option>
<option value=\"2025-05-20\">May 20 Tu, 2025</option>
<option value=\"2025-05-21\">May 21 We, 2025</option>
<option value=\"2025-05-22\">May 22 Th, 2025</option>
<option value=\"2025-05-23\">May 23 Fr, 2025</option>
<option value=\"2025-05-24\">May 24 Sa, 2025</option>
<option value=\"2025-05-25\">May 25 Su, 2025</option>
<option value=\"2025-05-26\">May 26 Mo, 2025</option>
<option value=\"2025-05-27\">May 27 Tu, 2025</option>
<option value=\"2025-05-28\">May 28 We, 2025</option>
<option value=\"2025-05-29\">May 29 Th, 2025</option>
<option value=\"2025-05-30\">May 30 Fr, 2025</option>
<option value=\"2025-05-31\">May 31 Sa, 2025</option>
<option value=\"2025-06-01\">Jun 01 Su, 2025</option>
<option value=\"2025-06-02\">Jun 02 Mo, 2025</option>
<option value=\"2025-06-03\">Jun 03 Tu, 2025</option>
<option value=\"2025-06-04\">Jun 04 We, 2025</option>
<option value=\"2025-06-05\">Jun 05 Th, 2025</option>
<option value=\"2025-06-06\">Jun 06 Fr, 2025</option>
<option value=\"2025-06-07\">Jun 07 Sa, 2025</option>
<option value=\"2025-06-08\">Jun 08 Su, 2025</option>
<option value=\"2025-06-09\">Jun 09 Mo, 2025</option>
<option value=\"2025-06-10\">Jun 10 Tu, 2025</option>
<option value=\"2025-06-11\">Jun 11 We, 2025</option>
<option value=\"2025-06-12\">Jun 12 Th, 2025</option>
<option value=\"2025-06-13\">Jun 13 Fr, 2025</option>
<option value=\"2025-06-14\">Jun 14 Sa, 2025</option>
<option value=\"2025-06-15\">Jun 15 Su, 2025</option>
<option value=\"2025-06-16\">Jun 16 Mo, 2025</option>
<option value=\"2025-06-17\">Jun 17 Tu, 2025</option>
<option value=\"2025-06-18\">Jun 18 We, 2025</option>
<option value=\"2025-06-19\">Jun 19 Th, 2025</option>
<option value=\"2025-06-20\">Jun 20 Fr, 2025</option>
<option value=\"2025-06-21\">Jun 21 Sa, 2025</option>
<option value=\"2025-06-22\">Jun 22 Su, 2025</option>
<option value=\"2025-06-23\">Jun 23 Mo, 2025</option>
<option value=\"2025-06-24\">Jun 24 Tu, 2025</option>
<option value=\"2025-06-25\">Jun 25 We, 2025</option>
<option value=\"2025-06-26\">Jun 26 Th, 2025</option>
<option value=\"2025-06-27\">Jun 27 Fr, 2025</option>
<option value=\"2025-06-28\">Jun 28 Sa, 2025</option>
<option value=\"2025-06-29\">Jun 29 Su, 2025</option>
<option value=\"2025-06-30\">Jun 30 Mo, 2025</option>
<option value=\"2025-07-01\">Jul 01 Tu, 2025</option>
<option value=\"2025-07-02\">Jul 02 We, 2025</option>
<option value=\"2025-07-03\">Jul 03 Th, 2025</option>
<option value=\"2025-07-04\">Jul 04 Fr, 2025</option>
<option value=\"2025-07-05\">Jul 05 Sa, 2025</option>
<option value=\"2025-07-06\">Jul 06 Su, 2025</option>
<option value=\"2025-07-07\">Jul 07 Mo, 2025</option>
<option value=\"2025-07-08\">Jul 08 Tu, 2025</option>
<option value=\"2025-07-09\">Jul 09 We, 2025</option>
<option value=\"2025-07-10\">Jul 10 Th, 2025</option>
<option value=\"2025-07-11\">Jul 11 Fr, 2025</option>
<option value=\"2025-07-12\">Jul 12 Sa, 2025</option>
<option value=\"2025-07-13\">Jul 13 Su, 2025</option>
<option value=\"2025-07-14\">Jul 14 Mo, 2025</option>
<option value=\"2025-07-15\">Jul 15 Tu, 2025</option>
<option value=\"2025-07-16\">Jul 16 We, 2025</option>
<option value=\"2025-07-17\">Jul 17 Th, 2025</option>
<option value=\"2025-07-18\">Jul 18 Fr, 2025</option>
<option value=\"2025-07-19\">Jul 19 Sa, 2025</option>
<option value=\"2025-07-20\">Jul 20 Su, 2025</option>
<option value=\"2025-07-21\">Jul 21 Mo, 2025</option>
<option value=\"2025-07-22\">Jul 22 Tu, 2025</option>
<option value=\"2025-07-23\">Jul 23 We, 2025</option>
<option value=\"2025-07-24\">Jul 24 Th, 2025</option>
<option value=\"2025-07-25\">Jul 25 Fr, 2025</option>
<option value=\"2025-07-26\">Jul 26 Sa, 2025</option>
<option value=\"2025-07-27\">Jul 27 Su, 2025</option>
<option value=\"2025-07-28\">Jul 28 Mo, 2025</option>
<option value=\"2025-07-29\">Jul 29 Tu, 2025</option>
<option value=\"2025-07-30\">Jul 30 We, 2025</option>
<option value=\"2025-07-31\">Jul 31 Th, 2025</option>
<option value=\"2025-08-01\">Aug 01 Fr, 2025</option>
<option value=\"2025-08-02\">Aug 02 Sa, 2025</option>
<option value=\"2025-08-03\">Aug 03 Su, 2025</option>
<option value=\"2025-08-04\">Aug 04 Mo, 2025</option>
<option value=\"2025-08-05\">Aug 05 Tu, 2025</option>
<option value=\"2025-08-06\">Aug 06 We, 2025</option>
<option value=\"2025-08-07\">Aug 07 Th, 2025</option>
<option value=\"2025-08-08\">Aug 08 Fr, 2025</option>
<option value=\"2025-08-09\">Aug 09 Sa, 2025</option>
<option value=\"2025-08-10\">Aug 10 Su, 2025</option>
<option value=\"2025-08-11\">Aug 11 Mo, 2025</option>
<option value=\"2025-08-12\">Aug 12 Tu, 2025</option>
<option value=\"2025-08-13\">Aug 13 We, 2025</option>
<option value=\"2025-08-14\">Aug 14 Th, 2025</option>
<option value=\"2025-08-15\">Aug 15 Fr, 2025</option>
<option value=\"2025-08-16\">Aug 16 Sa, 2025</option>
<option value=\"2025-08-17\">Aug 17 Su, 2025</option>
<option value=\"2025-08-18\">Aug 18 Mo, 2025</option>
<option value=\"2025-08-19\">Aug 19 Tu, 2025</option>
<option value=\"2025-08-20\">Aug 20 We, 2025</option>
<option value=\"2025-08-21\">Aug 21 Th, 2025</option>
<option value=\"2025-08-22\">Aug 22 Fr, 2025</option>
<option value=\"2025-08-23\">Aug 23 Sa, 2025</option>
<option value=\"2025-08-24\">Aug 24 Su, 2025</option>
<option value=\"2025-08-25\">Aug 25 Mo, 2025</option>
<option value=\"2025-08-26\">Aug 26 Tu, 2025</option>
<option value=\"2025-08-27\">Aug 27 We, 2025</option>
<option value=\"2025-08-28\">Aug 28 Th, 2025</option>
<option value=\"2025-08-29\">Aug 29 Fr, 2025</option>
<option value=\"2025-08-30\">Aug 30 Sa, 2025</option>
<option value=\"2025-08-31\">Aug 31 Su, 2025</option>
<option value=\"2025-09-01\">Sep 01 Mo, 2025</option>
<option value=\"2025-09-02\">Sep 02 Tu, 2025</option>
<option value=\"2025-09-03\">Sep 03 We, 2025</option>
<option value=\"2025-09-04\">Sep 04 Th, 2025</option>
<option value=\"2025-09-05\">Sep 05 Fr, 2025</option>
<option value=\"2025-09-06\">Sep 06 Sa, 2025</option>
<option value=\"2025-09-07\">Sep 07 Su, 2025</option>
<option value=\"2025-09-08\">Sep 08 Mo, 2025</option>
<option value=\"2025-09-09\">Sep 09 Tu, 2025</option>
<option value=\"2025-09-10\">Sep 10 We, 2025</option>
<option value=\"2025-09-11\">Sep 11 Th, 2025</option>
<option value=\"2025-09-12\">Sep 12 Fr, 2025</option>
<option value=\"2025-09-13\">Sep 13 Sa, 2025</option>
<option value=\"2025-09-14\">Sep 14 Su, 2025</option>
<option value=\"2025-09-15\">Sep 15 Mo, 2025</option>
<option value=\"2025-09-16\">Sep 16 Tu, 2025</option>
<option value=\"2025-09-17\">Sep 17 We, 2025</option>
<option value=\"2025-09-18\">Sep 18 Th, 2025</option>
<option value=\"2025-09-19\">Sep 19 Fr, 2025</option>
<option value=\"2025-09-20\">Sep 20 Sa, 2025</option>
<option value=\"2025-09-21\">Sep 21 Su, 2025</option>
<option value=\"2025-09-22\">Sep 22 Mo, 2025</option>
<option value=\"2025-09-23\">Sep 23 Tu, 2025</option>
<option value=\"2025-09-24\">Sep 24 We, 2025</option>
<option value=\"2025-09-25\">Sep 25 Th, 2025</option>
<option value=\"2025-09-26\">Sep 26 Fr, 2025</option>
<option value=\"2025-09-27\">Sep 27 Sa, 2025</option>
<option value=\"2025-09-28\">Sep 28 Su, 2025</option>
<option value=\"2025-09-29\">Sep 29 Mo, 2025</option>
<option value=\"2025-09-30\">Sep 30 Tu, 2025</option>
<option value=\"2025-10-01\">Oct 01 We, 2025</option>
<option value=\"2025-10-02\">Oct 02 Th, 2025</option>
<option value=\"2025-10-03\">Oct 03 Fr, 2025</option>
<option value=\"2025-10-04\">Oct 04 Sa, 2025</option>
<option value=\"2025-10-05\">Oct 05 Su, 2025</option>
<option value=\"2025-10-06\">Oct 06 Mo, 2025</option>
<option value=\"2025-10-07\">Oct 07 Tu, 2025</option>
<option value=\"2025-10-08\">Oct 08 We, 2025</option>
<option value=\"2025-10-09\">Oct 09 Th, 2025</option>
<option value=\"2025-10-10\">Oct 10 Fr, 2025</option>
<option value=\"2025-10-11\">Oct 11 Sa, 2025</option>
<option value=\"2025-10-12\">Oct 12 Su, 2025</option>
<option value=\"2025-10-13\">Oct 13 Mo, 2025</option>
<option value=\"2025-10-14\">Oct 14 Tu, 2025</option>
<option value=\"2025-10-15\">Oct 15 We, 2025</option>
<option value=\"2025-10-16\">Oct 16 Th, 2025</option>
<option value=\"2025-10-17\">Oct 17 Fr, 2025</option>
<option value=\"2025-10-18\">Oct 18 Sa, 2025</option>
<option value=\"2025-10-19\">Oct 19 Su, 2025</option>
<option value=\"2025-10-20\">Oct 20 Mo, 2025</option>
<option value=\"2025-10-21\">Oct 21 Tu, 2025</option>
<option value=\"2025-10-22\">Oct 22 We, 2025</option>
<option value=\"2025-10-23\">Oct 23 Th, 2025</option>
<option value=\"2025-10-24\">Oct 24 Fr, 2025</option>
<option value=\"2025-10-25\">Oct 25 Sa, 2025</option>
<option value=\"2025-10-26\">Oct 26 Su, 2025</option>
<option value=\"2025-10-27\">Oct 27 Mo, 2025</option>
<option value=\"2025-10-28\">Oct 28 Tu, 2025</option>
<option value=\"2025-10-29\">Oct 29 We, 2025</option>
<option value=\"2025-10-30\">Oct 30 Th, 2025</option>
<option value=\"2025-10-31\">Oct 31 Fr, 2025</option>
<option value=\"2025-11-01\">Nov 01 Sa, 2025</option>
<option value=\"2025-11-02\">Nov 02 Su, 2025</option>
<option value=\"2025-11-03\">Nov 03 Mo, 2025</option>
<option value=\"2025-11-04\">Nov 04 Tu, 2025</option>
<option value=\"2025-11-05\">Nov 05 We, 2025</option>
<option value=\"2025-11-06\">Nov 06 Th, 2025</option>
<option value=\"2025-11-07\">Nov 07 Fr, 2025</option>
<option value=\"2025-11-08\">Nov 08 Sa, 2025</option>
<option value=\"2025-11-09\">Nov 09 Su, 2025</option>
<option value=\"2025-11-10\">Nov 10 Mo, 2025</option>
<option value=\"2025-11-11\">Nov 11 Tu, 2025</option>
<option value=\"2025-11-12\">Nov 12 We, 2025</option>
<option value=\"2025-11-13\">Nov 13 Th, 2025</option>
<option value=\"2025-11-14\">Nov 14 Fr, 2025</option>
<option value=\"2025-11-15\">Nov 15 Sa, 2025</option>
<option value=\"2025-11-16\">Nov 16 Su, 2025</option>
<option value=\"2025-11-17\">Nov 17 Mo, 2025</option>
<option value=\"2025-11-18\">Nov 18 Tu, 2025</option>
<option value=\"2025-11-19\">Nov 19 We, 2025</option>
<option value=\"2025-11-20\">Nov 20 Th, 2025</option>
<option value=\"2025-11-21\">Nov 21 Fr, 2025</option>
<option value=\"2025-11-22\">Nov 22 Sa, 2025</option>
<option value=\"2025-11-23\">Nov 23 Su, 2025</option>
<option value=\"2025-11-24\">Nov 24 Mo, 2025</option>
<option value=\"2025-11-25\">Nov 25 Tu, 2025</option>
<option value=\"2025-11-26\">Nov 26 We, 2025</option>
<option value=\"2025-11-27\">Nov 27 Th, 2025</option>
<option value=\"2025-11-28\">Nov 28 Fr, 2025</option>
<option value=\"2025-11-29\">Nov 29 Sa, 2025</option>
<option value=\"2025-11-30\">Nov 30 Su, 2025</option>
<option value=\"2025-12-01\">Dec 01 Mo, 2025</option>
<option value=\"2025-12-02\">Dec 02 Tu, 2025</option>
<option value=\"2025-12-03\">Dec 03 We, 2025</option>
<option value=\"2025-12-04\">Dec 04 Th, 2025</option>
<option value=\"2025-12-05\">Dec 05 Fr, 2025</option>
<option value=\"2025-12-06\">Dec 06 Sa, 2025</option>
<option value=\"2025-12-07\">Dec 07 Su, 2025</option>
<option value=\"2025-12-08\">Dec 08 Mo, 2025</option>
<option value=\"2025-12-09\">Dec 09 Tu, 2025</option>
<option value=\"2025-12-10\">Dec 10 We, 2025</option>
<option value=\"2025-12-11\">Dec 11 Th, 2025</option>
<option value=\"2025-12-12\">Dec 12 Fr, 2025</option>
<option value=\"2025-12-13\">Dec 13 Sa, 2025</option>
<option value=\"2025-12-14\">Dec 14 Su, 2025</option>
<option value=\"2025-12-15\">Dec 15 Mo, 2025</option>
<option value=\"2025-12-16\">Dec 16 Tu, 2025</option>
<option value=\"2025-12-17\">Dec 17 We, 2025</option>
<option value=\"2025-12-18\">Dec 18 Th, 2025</option>
<option value=\"2025-12-19\">Dec 19 Fr, 2025</option>
<option value=\"2025-12-20\">Dec 20 Sa, 2025</option>
<option value=\"2025-12-21\">Dec 21 Su, 2025</option>
<option value=\"2025-12-22\">Dec 22 Mo, 2025</option>
<option value=\"2025-12-23\">Dec 23 Tu, 2025</option>
<option value=\"2025-12-24\">Dec 24 We, 2025</option>
<option value=\"2025-12-25\">Dec 25 Th, 2025</option>
<option value=\"2025-12-26\">Dec 26 Fr, 2025</option>
<option value=\"2025-12-27\">Dec 27 Sa, 2025</option>
<option value=\"2025-12-28\">Dec 28 Su, 2025</option>
<option value=\"2025-12-29\">Dec 29 Mo, 2025</option>
<option value=\"2025-12-30\">Dec 30 Tu, 2025</option>
<option value=\"2025-12-31\">Dec 31 We, 2025</option>
<option value=\"2026-01-01\">Jan 01 Th, 2026</option>
<option value=\"2026-01-02\">Jan 02 Fr, 2026</option>
<option value=\"2026-01-03\">Jan 03 Sa, 2026</option>
<option value=\"2026-01-04\">Jan 04 Su, 2026</option>
<option value=\"2026-01-05\">Jan 05 Mo, 2026</option>
<option value=\"2026-01-06\">Jan 06 Tu, 2026</option>
<option value=\"2026-01-07\">Jan 07 We, 2026</option>
<option value=\"2026-01-08\">Jan 08 Th, 2026</option>
<option value=\"2026-01-09\">Jan 09 Fr, 2026</option>
<option value=\"2026-01-10\">Jan 10 Sa, 2026</option>
<option value=\"2026-01-11\">Jan 11 Su, 2026</option>
<option value=\"2026-01-12\">Jan 12 Mo, 2026</option>
<option value=\"2026-01-13\">Jan 13 Tu, 2026</option>
<option value=\"2026-01-14\">Jan 14 We, 2026</option>
<option value=\"2026-01-15\">Jan 15 Th, 2026</option>
<option value=\"2026-01-16\">Jan 16 Fr, 2026</option>
<option value=\"2026-01-17\">Jan 17 Sa, 2026</option>
<option value=\"2026-01-18\">Jan 18 Su, 2026</option>
<option value=\"2026-01-19\">Jan 19 Mo, 2026</option>
<option value=\"2026-01-20\">Jan 20 Tu, 2026</option>
<option value=\"2026-01-21\">Jan 21 We, 2026</option>
<option value=\"2026-01-22\">Jan 22 Th, 2026</option>
<option value=\"2026-01-23\">Jan 23 Fr, 2026</option>
<option value=\"2026-01-24\">Jan 24 Sa, 2026</option>
<option value=\"2026-01-25\">Jan 25 Su, 2026</option>
<option value=\"2026-01-26\">Jan 26 Mo, 2026</option>
<option value=\"2026-01-27\">Jan 27 Tu, 2026</option>
<option value=\"2026-01-28\">Jan 28 We, 2026</option>
<option value=\"2026-01-29\">Jan 29 Th, 2026</option>
<option value=\"2026-01-30\">Jan 30 Fr, 2026</option>
<option value=\"2026-01-31\">Jan 31 Sa, 2026</option>
<option value=\"2026-02-01\">Feb 01 Su, 2026</option>
<option value=\"2026-02-02\">Feb 02 Mo, 2026</option>
<option value=\"2026-02-03\">Feb 03 Tu, 2026</option>
<option value=\"2026-02-04\">Feb 04 We, 2026</option>
<option value=\"2026-02-05\">Feb 05 Th, 2026</option>
<option value=\"2026-02-06\">Feb 06 Fr, 2026</option>
<option value=\"2026-02-07\">Feb 07 Sa, 2026</option>
<option value=\"2026-02-08\">Feb 08 Su, 2026</option>
<option value=\"2026-02-09\">Feb 09 Mo, 2026</option>
<option value=\"2026-02-10\">Feb 10 Tu, 2026</option>
<option value=\"2026-02-11\">Feb 11 We, 2026</option>
<option value=\"2026-02-12\">Feb 12 Th, 2026</option>
<option value=\"2026-02-13\">Feb 13 Fr, 2026</option>
<option value=\"2026-02-14\">Feb 14 Sa, 2026</option>
<option value=\"2026-02-15\">Feb 15 Su, 2026</option>
<option value=\"2026-02-16\">Feb 16 Mo, 2026</option>
<option value=\"2026-02-17\">Feb 17 Tu, 2026</option>
<option value=\"2026-02-18\">Feb 18 We, 2026</option>
<option value=\"2026-02-19\">Feb 19 Th, 2026</option>
<option value=\"2026-02-20\">Feb 20 Fr, 2026</option>
<option value=\"2026-02-21\">Feb 21 Sa, 2026</option>
<option value=\"2026-02-22\">Feb 22 Su, 2026</option>
<option value=\"2026-02-23\">Feb 23 Mo, 2026</option>
<option value=\"2026-02-24\">Feb 24 Tu, 2026</option>
<option value=\"2026-02-25\">Feb 25 We, 2026</option>
<option value=\"2026-02-26\">Feb 26 Th, 2026</option>
<option value=\"2026-02-27\">Feb 27 Fr, 2026</option>
<option value=\"2026-02-28\">Feb 28 Sa, 2026</option>
<option value=\"2026-03-01\">Mar 01 Su, 2026</option>
<option value=\"2026-03-02\">Mar 02 Mo, 2026</option>
<option value=\"2026-03-03\">Mar 03 Tu, 2026</option>
<option value=\"2026-03-04\">Mar 04 We, 2026</option>
<option value=\"2026-03-05\">Mar 05 Th, 2026</option>
<option value=\"2026-03-06\">Mar 06 Fr, 2026</option>
<option value=\"2026-03-07\">Mar 07 Sa, 2026</option>
<option value=\"2026-03-08\">Mar 08 Su, 2026</option>
<option value=\"2026-03-09\">Mar 09 Mo, 2026</option>
<option value=\"2026-03-10\">Mar 10 Tu, 2026</option>
<option value=\"2026-03-11\">Mar 11 We, 2026</option>
<option value=\"2026-03-12\">Mar 12 Th, 2026</option>
<option value=\"2026-03-13\">Mar 13 Fr, 2026</option>
<option value=\"2026-03-14\">Mar 14 Sa, 2026</option>
<option value=\"2026-03-15\">Mar 15 Su, 2026</option>
<option value=\"2026-03-16\">Mar 16 Mo, 2026</option>
<option value=\"2026-03-17\">Mar 17 Tu, 2026</option>
<option value=\"2026-03-18\">Mar 18 We, 2026</option>
<option value=\"2026-03-19\">Mar 19 Th, 2026</option>
<option value=\"2026-03-20\">Mar 20 Fr, 2026</option>
<option value=\"2026-03-21\">Mar 21 Sa, 2026</option>
<option value=\"2026-03-22\">Mar 22 Su, 2026</option>
<option value=\"2026-03-23\">Mar 23 Mo, 2026</option>
<option value=\"2026-03-24\">Mar 24 Tu, 2026</option>
<option value=\"2026-03-25\">Mar 25 We, 2026</option>
<option value=\"2026-03-26\">Mar 26 Th, 2026</option>
<option value=\"2026-03-27\">Mar 27 Fr, 2026</option>
<option value=\"2026-03-28\">Mar 28 Sa, 2026</option>
<option value=\"2026-03-29\">Mar 29 Su, 2026</option>
<option value=\"2026-03-30\">Mar 30 Mo, 2026</option>
<option value=\"2026-03-31\">Mar 31 Tu, 2026</option>
<option value=\"2026-04-01\">Apr 01 We, 2026</option>
<option value=\"2026-04-02\">Apr 02 Th, 2026</option>
<option value=\"2026-04-03\">Apr 03 Fr, 2026</option>
<option value=\"2026-04-04\">Apr 04 Sa, 2026</option>
<option value=\"2026-04-05\">Apr 05 Su, 2026</option>
<option value=\"2026-04-06\">Apr 06 Mo, 2026</option>
<option value=\"2026-04-07\">Apr 07 Tu, 2026</option>
<option value=\"2026-04-08\">Apr 08 We, 2026</option>
<option value=\"2026-04-09\">Apr 09 Th, 2026</option>
<option value=\"2026-04-10\">Apr 10 Fr, 2026</option>
<option value=\"2026-04-11\">Apr 11 Sa, 2026</option>
<option value=\"2026-04-12\">Apr 12 Su, 2026</option>
<option value=\"2026-04-13\">Apr 13 Mo, 2026</option>
<option value=\"2026-04-14\">Apr 14 Tu, 2026</option>
<option value=\"2026-04-15\">Apr 15 We, 2026</option>
<option value=\"2026-04-16\">Apr 16 Th, 2026</option>
<option value=\"2026-04-17\">Apr 17 Fr, 2026</option>
<option value=\"2026-04-18\">Apr 18 Sa, 2026</option>
<option value=\"2026-04-19\">Apr 19 Su, 2026</option>
<option value=\"2026-04-20\">Apr 20 Mo, 2026</option>
<option value=\"2026-04-21\">Apr 21 Tu, 2026</option>
<option value=\"2026-04-22\">Apr 22 We, 2026</option>
<option value=\"2026-04-23\">Apr 23 Th, 2026</option>
<option value=\"2026-04-24\">Apr 24 Fr, 2026</option>
<option value=\"2026-04-25\">Apr 25 Sa, 2026</option>
<option value=\"2026-04-26\">Apr 26 Su, 2026</option>
<option value=\"2026-04-27\">Apr 27 Mo, 2026</option>
<option value=\"2026-04-28\">Apr 28 Tu, 2026</option>
<option value=\"2026-04-29\">Apr 29 We, 2026</option>
<option value=\"2026-04-30\">Apr 30 Th, 2026</option>
<option value=\"2026-05-01\">May 01 Fr, 2026</option>
<option value=\"2026-05-02\">May 02 Sa, 2026</option>
<option value=\"2026-05-03\">May 03 Su, 2026</option>
<option value=\"2026-05-04\">May 04 Mo, 2026</option>
<option value=\"2026-05-05\">May 05 Tu, 2026</option>
<option value=\"2026-05-06\">May 06 We, 2026</option>
<option value=\"2026-05-07\">May 07 Th, 2026</option>
<option value=\"2026-05-08\">May 08 Fr, 2026</option>
<option value=\"2026-05-09\">May 09 Sa, 2026</option>
<option value=\"2026-05-10\">May 10 Su, 2026</option>
<option value=\"2026-05-11\">May 11 Mo, 2026</option>
<option value=\"2026-05-12\">May 12 Tu, 2026</option>
<option value=\"2026-05-13\">May 13 We, 2026</option>
<option value=\"2026-05-14\">May 14 Th, 2026</option>
<option value=\"2026-05-15\">May 15 Fr, 2026</option>
<option value=\"2026-05-16\">May 16 Sa, 2026</option>
<option value=\"2026-05-17\">May 17 Su, 2026</option>
<option value=\"2026-05-18\">May 18 Mo, 2026</option>
<option value=\"2026-05-19\">May 19 Tu, 2026</option>
<option value=\"2026-05-20\">May 20 We, 2026</option>
<option value=\"2026-05-21\">May 21 Th, 2026</option>
<option value=\"2026-05-22\">May 22 Fr, 2026</option>
<option value=\"2026-05-23\">May 23 Sa, 2026</option>
<option value=\"2026-05-24\">May 24 Su, 2026</option>
<option value=\"2026-05-25\">May 25 Mo, 2026</option>
<option value=\"2026-05-26\">May 26 Tu, 2026</option>
<option value=\"2026-05-27\">May 27 We, 2026</option>
<option value=\"2026-05-28\">May 28 Th, 2026</option>
<option value=\"2026-05-29\">May 29 Fr, 2026</option>
<option value=\"2026-05-30\">May 30 Sa, 2026</option>
<option value=\"2026-05-31\">May 31 Su, 2026</option>
<option value=\"2026-06-01\">Jun 01 Mo, 2026</option>
<option value=\"2026-06-02\">Jun 02 Tu, 2026</option>
<option value=\"2026-06-03\">Jun 03 We, 2026</option>
<option value=\"2026-06-04\">Jun 04 Th, 2026</option>
<option value=\"2026-06-05\">Jun 05 Fr, 2026</option>
<option value=\"2026-06-06\">Jun 06 Sa, 2026</option>
<option value=\"2026-06-07\">Jun 07 Su, 2026</option>
<option value=\"2026-06-08\">Jun 08 Mo, 2026</option>
<option value=\"2026-06-09\">Jun 09 Tu, 2026</option>
<option value=\"2026-06-10\">Jun 10 We, 2026</option>
<option value=\"2026-06-11\">Jun 11 Th, 2026</option>
<option value=\"2026-06-12\">Jun 12 Fr, 2026</option>
<option value=\"2026-06-13\">Jun 13 Sa, 2026</option>
<option value=\"2026-06-14\">Jun 14 Su, 2026</option>
<option value=\"2026-06-15\">Jun 15 Mo, 2026</option>
<option value=\"2026-06-16\">Jun 16 Tu, 2026</option>
<option value=\"2026-06-17\">Jun 17 We, 2026</option>
<option value=\"2026-06-18\">Jun 18 Th, 2026</option>
<option value=\"2026-06-19\">Jun 19 Fr, 2026</option>
<option value=\"2026-06-20\">Jun 20 Sa, 2026</option>
<option value=\"2026-06-21\">Jun 21 Su, 2026</option>
<option value=\"2026-06-22\">Jun 22 Mo, 2026</option>
<option value=\"2026-06-23\">Jun 23 Tu, 2026</option>
<option value=\"2026-06-24\">Jun 24 We, 2026</option>
<option value=\"2026-06-25\">Jun 25 Th, 2026</option>
<option value=\"2026-06-26\">Jun 26 Fr, 2026</option>
<option value=\"2026-06-27\">Jun 27 Sa, 2026</option>
<option value=\"2026-06-28\">Jun 28 Su, 2026</option>
<option value=\"2026-06-29\">Jun 29 Mo, 2026</option>
<option value=\"2026-06-30\">Jun 30 Tu, 2026</option>
<option value=\"2026-07-01\">Jul 01 We, 2026</option>
<option value=\"2026-07-02\">Jul 02 Th, 2026</option>
<option value=\"2026-07-03\">Jul 03 Fr, 2026</option>
<option value=\"2026-07-04\">Jul 04 Sa, 2026</option>
<option value=\"2026-07-05\">Jul 05 Su, 2026</option>
<option value=\"2026-07-06\">Jul 06 Mo, 2026</option>
<option value=\"2026-07-07\">Jul 07 Tu, 2026</option>
<option value=\"2026-07-08\">Jul 08 We, 2026</option>
<option value=\"2026-07-09\">Jul 09 Th, 2026</option>
<option value=\"2026-07-10\">Jul 10 Fr, 2026</option>
<option value=\"2026-07-11\">Jul 11 Sa, 2026</option>
<option value=\"2026-07-12\">Jul 12 Su, 2026</option>
<option value=\"2026-07-13\">Jul 13 Mo, 2026</option>
<option value=\"2026-07-14\">Jul 14 Tu, 2026</option>
<option value=\"2026-07-15\">Jul 15 We, 2026</option>
<option value=\"2026-07-16\">Jul 16 Th, 2026</option>
<option value=\"2026-07-17\">Jul 17 Fr, 2026</option>
<option value=\"2026-07-18\">Jul 18 Sa, 2026</option>
<option value=\"2026-07-19\">Jul 19 Su, 2026</option>
<option value=\"2026-07-20\">Jul 20 Mo, 2026</option>
<option value=\"2026-07-21\">Jul 21 Tu, 2026</option>
<option value=\"2026-07-22\">Jul 22 We, 2026</option>
<option value=\"2026-07-23\">Jul 23 Th, 2026</option>
<option value=\"2026-07-24\">Jul 24 Fr, 2026</option>
<option value=\"2026-07-25\">Jul 25 Sa, 2026</option>
<option value=\"2026-07-26\">Jul 26 Su, 2026</option>
<option value=\"2026-07-27\">Jul 27 Mo, 2026</option>
<option value=\"2026-07-28\">Jul 28 Tu, 2026</option>
<option value=\"2026-07-29\">Jul 29 We, 2026</option>
<option value=\"2026-07-30\">Jul 30 Th, 2026</option>
<option value=\"2026-07-31\">Jul 31 Fr, 2026</option>
<option value=\"2026-08-01\">Aug 01 Sa, 2026</option>
<option value=\"2026-08-02\">Aug 02 Su, 2026</option>
<option value=\"2026-08-03\">Aug 03 Mo, 2026</option>
<option value=\"2026-08-04\">Aug 04 Tu, 2026</option>
<option value=\"2026-08-05\">Aug 05 We, 2026</option>
<option value=\"2026-08-06\">Aug 06 Th, 2026</option>
<option value=\"2026-08-07\">Aug 07 Fr, 2026</option>
<option value=\"2026-08-08\">Aug 08 Sa, 2026</option>
<option value=\"2026-08-09\">Aug 09 Su, 2026</option>
<option value=\"2026-08-10\">Aug 10 Mo, 2026</option>
<option value=\"2026-08-11\">Aug 11 Tu, 2026</option>
<option value=\"2026-08-12\">Aug 12 We, 2026</option>
<option value=\"2026-08-13\">Aug 13 Th, 2026</option>
<option value=\"2026-08-14\">Aug 14 Fr, 2026</option>
<option value=\"2026-08-15\">Aug 15 Sa, 2026</option>
<option value=\"2026-08-16\">Aug 16 Su, 2026</option>
<option value=\"2026-08-17\">Aug 17 Mo, 2026</option>
<option value=\"2026-08-18\">Aug 18 Tu, 2026</option>
<option value=\"2026-08-19\">Aug 19 We, 2026</option>
<option value=\"2026-08-20\">Aug 20 Th, 2026</option>
<option value=\"2026-08-21\">Aug 21 Fr, 2026</option>
<option value=\"2026-08-22\">Aug 22 Sa, 2026</option>
<option value=\"2026-08-23\">Aug 23 Su, 2026</option>
<option value=\"2026-08-24\">Aug 24 Mo, 2026</option>
<option value=\"2026-08-25\">Aug 25 Tu, 2026</option>
<option value=\"2026-08-26\">Aug 26 We, 2026</option>
<option value=\"2026-08-27\">Aug 27 Th, 2026</option>
<option value=\"2026-08-28\">Aug 28 Fr, 2026</option>
<option value=\"2026-08-29\">Aug 29 Sa, 2026</option>
<option value=\"2026-08-30\">Aug 30 Su, 2026</option>
<option value=\"2026-08-31\">Aug 31 Mo, 2026</option>
<option value=\"2026-09-01\">Sep 01 Tu, 2026</option>
<option value=\"2026-09-02\">Sep 02 We, 2026</option>
<option value=\"2026-09-03\">Sep 03 Th, 2026</option>
<option value=\"2026-09-04\">Sep 04 Fr, 2026</option>
<option value=\"2026-09-05\">Sep 05 Sa, 2026</option>
<option value=\"2026-09-06\">Sep 06 Su, 2026</option>
<option value=\"2026-09-07\">Sep 07 Mo, 2026</option>
<option value=\"2026-09-08\">Sep 08 Tu, 2026</option>
<option value=\"2026-09-09\">Sep 09 We, 2026</option>
<option value=\"2026-09-10\">Sep 10 Th, 2026</option>
<option value=\"2026-09-11\">Sep 11 Fr, 2026</option>
<option value=\"2026-09-12\">Sep 12 Sa, 2026</option>
<option value=\"2026-09-13\">Sep 13 Su, 2026</option>
<option value=\"2026-09-14\">Sep 14 Mo, 2026</option>
<option value=\"2026-09-15\">Sep 15 Tu, 2026</option>
<option value=\"2026-09-16\">Sep 16 We, 2026</option>
<option value=\"2026-09-17\">Sep 17 Th, 2026</option>
<option value=\"2026-09-18\">Sep 18 Fr, 2026</option>
<option value=\"2026-09-19\">Sep 19 Sa, 2026</option>
<option value=\"2026-09-20\">Sep 20 Su, 2026</option>
<option value=\"2026-09-21\">Sep 21 Mo, 2026</option>
<option value=\"2026-09-22\">Sep 22 Tu, 2026</option>
<option value=\"2026-09-23\">Sep 23 We, 2026</option>
<option value=\"2026-09-24\">Sep 24 Th, 2026</option>
<option value=\"2026-09-25\">Sep 25 Fr, 2026</option>
<option value=\"2026-09-26\">Sep 26 Sa, 2026</option>
<option value=\"2026-09-27\">Sep 27 Su, 2026</option>
<option value=\"2026-09-28\">Sep 28 Mo, 2026</option>
<option value=\"2026-09-29\">Sep 29 Tu, 2026</option>
<option value=\"2026-09-30\">Sep 30 We, 2026</option>
<option value=\"2026-10-01\">Oct 01 Th, 2026</option>
<option value=\"2026-10-02\">Oct 02 Fr, 2026</option>
<option value=\"2026-10-03\">Oct 03 Sa, 2026</option>
<option value=\"2026-10-04\">Oct 04 Su, 2026</option>
<option value=\"2026-10-05\">Oct 05 Mo, 2026</option>
<option value=\"2026-10-06\">Oct 06 Tu, 2026</option>
<option value=\"2026-10-07\">Oct 07 We, 2026</option>
<option value=\"2026-10-08\">Oct 08 Th, 2026</option>
<option value=\"2026-10-09\">Oct 09 Fr, 2026</option>
<option value=\"2026-10-10\">Oct 10 Sa, 2026</option>
<option value=\"2026-10-11\">Oct 11 Su, 2026</option>
<option value=\"2026-10-12\">Oct 12 Mo, 2026</option>
<option value=\"2026-10-13\">Oct 13 Tu, 2026</option>
<option value=\"2026-10-14\">Oct 14 We, 2026</option>
<option value=\"2026-10-15\">Oct 15 Th, 2026</option>
<option value=\"2026-10-16\">Oct 16 Fr, 2026</option>
<option value=\"2026-10-17\">Oct 17 Sa, 2026</option>
<option value=\"2026-10-18\">Oct 18 Su, 2026</option>
<option value=\"2026-10-19\">Oct 19 Mo, 2026</option>
<option value=\"2026-10-20\">Oct 20 Tu, 2026</option>
<option value=\"2026-10-21\">Oct 21 We, 2026</option>
<option value=\"2026-10-22\">Oct 22 Th, 2026</option>
<option value=\"2026-10-23\">Oct 23 Fr, 2026</option>
<option value=\"2026-10-24\">Oct 24 Sa, 2026</option>
<option value=\"2026-10-25\">Oct 25 Su, 2026</option>
<option value=\"2026-10-26\">Oct 26 Mo, 2026</option>
<option value=\"2026-10-27\">Oct 27 Tu, 2026</option>
<option value=\"2026-10-28\">Oct 28 We, 2026</option>
<option value=\"2026-10-29\">Oct 29 Th, 2026</option>
<option value=\"2026-10-30\">Oct 30 Fr, 2026</option>
<option value=\"2026-10-31\">Oct 31 Sa, 2026</option>
<option value=\"2026-11-01\">Nov 01 Su, 2026</option>
<option value=\"2026-11-02\">Nov 02 Mo, 2026</option>
<option value=\"2026-11-03\">Nov 03 Tu, 2026</option>
<option value=\"2026-11-04\">Nov 04 We, 2026</option>
<option value=\"2026-11-05\">Nov 05 Th, 2026</option>
<option value=\"2026-11-06\">Nov 06 Fr, 2026</option>
<option value=\"2026-11-07\">Nov 07 Sa, 2026</option>
<option value=\"2026-11-08\">Nov 08 Su, 2026</option>
<option value=\"2026-11-09\">Nov 09 Mo, 2026</option>
<option value=\"2026-11-10\">Nov 10 Tu, 2026</option>
<option value=\"2026-11-11\">Nov 11 We, 2026</option>
<option value=\"2026-11-12\">Nov 12 Th, 2026</option>
<option value=\"2026-11-13\">Nov 13 Fr, 2026</option>
<option value=\"2026-11-14\">Nov 14 Sa, 2026</option>
<option value=\"2026-11-15\">Nov 15 Su, 2026</option>
<option value=\"2026-11-16\">Nov 16 Mo, 2026</option>
<option value=\"2026-11-17\">Nov 17 Tu, 2026</option>
<option value=\"2026-11-18\">Nov 18 We, 2026</option>
<option value=\"2026-11-19\">Nov 19 Th, 2026</option>
<option value=\"2026-11-20\">Nov 20 Fr, 2026</option>
<option value=\"2026-11-21\">Nov 21 Sa, 2026</option>
<option value=\"2026-11-22\">Nov 22 Su, 2026</option>
<option value=\"2026-11-23\">Nov 23 Mo, 2026</option>
<option value=\"2026-11-24\">Nov 24 Tu, 2026</option>
<option value=\"2026-11-25\">Nov 25 We, 2026</option>
<option value=\"2026-11-26\">Nov 26 Th, 2026</option>
<option value=\"2026-11-27\">Nov 27 Fr, 2026</option>
<option value=\"2026-11-28\">Nov 28 Sa, 2026</option>
<option value=\"2026-11-29\">Nov 29 Su, 2026</option>
<option value=\"2026-11-30\">Nov 30 Mo, 2026</option>
<option value=\"2026-12-01\">Dec 01 Tu, 2026</option>
<option value=\"2026-12-02\">Dec 02 We, 2026</option>
<option value=\"2026-12-03\">Dec 03 Th, 2026</option>
<option value=\"2026-12-04\">Dec 04 Fr, 2026</option>
<option value=\"2026-12-05\">Dec 05 Sa, 2026</option>
<option value=\"2026-12-06\">Dec 06 Su, 2026</option>
<option value=\"2026-12-07\">Dec 07 Mo, 2026</option>
<option value=\"2026-12-08\">Dec 08 Tu, 2026</option>
<option value=\"2026-12-09\">Dec 09 We, 2026</option>
<option value=\"2026-12-10\">Dec 10 Th, 2026</option>
<option value=\"2026-12-11\">Dec 11 Fr, 2026</option>
<option value=\"2026-12-12\">Dec 12 Sa, 2026</option>
<option value=\"2026-12-13\">Dec 13 Su, 2026</option>
<option value=\"2026-12-14\">Dec 14 Mo, 2026</option>
<option value=\"2026-12-15\">Dec 15 Tu, 2026</option>
<option value=\"2026-12-16\">Dec 16 We, 2026</option>
<option value=\"2026-12-17\">Dec 17 Th, 2026</option>
<option value=\"2026-12-18\">Dec 18 Fr, 2026</option>
<option value=\"2026-12-19\">Dec 19 Sa, 2026</option>
<option value=\"2026-12-20\">Dec 20 Su, 2026</option>
<option value=\"2026-12-21\">Dec 21 Mo, 2026</option>
<option value=\"2026-12-22\">Dec 22 Tu, 2026</option>
<option value=\"2026-12-23\">Dec 23 We, 2026</option>
<option value=\"2026-12-24\">Dec 24 Th, 2026</option>
<option value=\"2026-12-25\">Dec 25 Fr, 2026</option>
<option value=\"2026-12-26\">Dec 26 Sa, 2026</option>
<option value=\"2026-12-27\">Dec 27 Su, 2026</option>
<option value=\"2026-12-28\">Dec 28 Mo, 2026</option>
<option value=\"2026-12-29\">Dec 29 Tu, 2026</option>
<option value=\"2026-12-30\">Dec 30 We, 2026</option>
<option value=\"2026-12-31\">Dec 31 Th, 2026</option>
<option value=\"2027-01-01\">Jan 01 Fr, 2027</option>

";

?></contenuto>
</file>
<file>
<nomefile>./dati/selperiodimenu2025.1.php</nomefile>
<contenuto>
<?php 

$y_ini_menu = array();
$m_ini_menu = array();
$d_ini_menu = array();
$n_dates_menu = array();
$d_increment = array();
$y_ini_menu[0] = "2025";
$m_ini_menu[0] = "0";
$d_ini_menu[0] = "01";
$n_dates_menu[0] = "731";
$d_increment[0] = "1";
$d_names = "\" Su\",\" Mo\",\" Tu\",\" We\",\" Th\",\" Fr\",\" Sa\"";
$m_names = "\"Jan\",\"Feb\",\"Mar\",\"Apr\",\"May\",\"Jun\",\"Jul\",\"Aug\",\"Sep\",\"Oct\",\"Nov\",\"Dec\"";

$dates_options_list = "

<option value=\"2025-01-01\">Jan 01 We, 2025</option>
<option value=\"2025-01-02\">Jan 02 Th, 2025</option>
<option value=\"2025-01-03\">Jan 03 Fr, 2025</option>
<option value=\"2025-01-04\">Jan 04 Sa, 2025</option>
<option value=\"2025-01-05\">Jan 05 Su, 2025</option>
<option value=\"2025-01-06\">Jan 06 Mo, 2025</option>
<option value=\"2025-01-07\">Jan 07 Tu, 2025</option>
<option value=\"2025-01-08\">Jan 08 We, 2025</option>
<option value=\"2025-01-09\">Jan 09 Th, 2025</option>
<option value=\"2025-01-10\">Jan 10 Fr, 2025</option>
<option value=\"2025-01-11\">Jan 11 Sa, 2025</option>
<option value=\"2025-01-12\">Jan 12 Su, 2025</option>
<option value=\"2025-01-13\">Jan 13 Mo, 2025</option>
<option value=\"2025-01-14\">Jan 14 Tu, 2025</option>
<option value=\"2025-01-15\">Jan 15 We, 2025</option>
<option value=\"2025-01-16\">Jan 16 Th, 2025</option>
<option value=\"2025-01-17\">Jan 17 Fr, 2025</option>
<option value=\"2025-01-18\">Jan 18 Sa, 2025</option>
<option value=\"2025-01-19\">Jan 19 Su, 2025</option>
<option value=\"2025-01-20\">Jan 20 Mo, 2025</option>
<option value=\"2025-01-21\">Jan 21 Tu, 2025</option>
<option value=\"2025-01-22\">Jan 22 We, 2025</option>
<option value=\"2025-01-23\">Jan 23 Th, 2025</option>
<option value=\"2025-01-24\">Jan 24 Fr, 2025</option>
<option value=\"2025-01-25\">Jan 25 Sa, 2025</option>
<option value=\"2025-01-26\">Jan 26 Su, 2025</option>
<option value=\"2025-01-27\">Jan 27 Mo, 2025</option>
<option value=\"2025-01-28\">Jan 28 Tu, 2025</option>
<option value=\"2025-01-29\">Jan 29 We, 2025</option>
<option value=\"2025-01-30\">Jan 30 Th, 2025</option>
<option value=\"2025-01-31\">Jan 31 Fr, 2025</option>
<option value=\"2025-02-01\">Feb 01 Sa, 2025</option>
<option value=\"2025-02-02\">Feb 02 Su, 2025</option>
<option value=\"2025-02-03\">Feb 03 Mo, 2025</option>
<option value=\"2025-02-04\">Feb 04 Tu, 2025</option>
<option value=\"2025-02-05\">Feb 05 We, 2025</option>
<option value=\"2025-02-06\">Feb 06 Th, 2025</option>
<option value=\"2025-02-07\">Feb 07 Fr, 2025</option>
<option value=\"2025-02-08\">Feb 08 Sa, 2025</option>
<option value=\"2025-02-09\">Feb 09 Su, 2025</option>
<option value=\"2025-02-10\">Feb 10 Mo, 2025</option>
<option value=\"2025-02-11\">Feb 11 Tu, 2025</option>
<option value=\"2025-02-12\">Feb 12 We, 2025</option>
<option value=\"2025-02-13\">Feb 13 Th, 2025</option>
<option value=\"2025-02-14\">Feb 14 Fr, 2025</option>
<option value=\"2025-02-15\">Feb 15 Sa, 2025</option>
<option value=\"2025-02-16\">Feb 16 Su, 2025</option>
<option value=\"2025-02-17\">Feb 17 Mo, 2025</option>
<option value=\"2025-02-18\">Feb 18 Tu, 2025</option>
<option value=\"2025-02-19\">Feb 19 We, 2025</option>
<option value=\"2025-02-20\">Feb 20 Th, 2025</option>
<option value=\"2025-02-21\">Feb 21 Fr, 2025</option>
<option value=\"2025-02-22\">Feb 22 Sa, 2025</option>
<option value=\"2025-02-23\">Feb 23 Su, 2025</option>
<option value=\"2025-02-24\">Feb 24 Mo, 2025</option>
<option value=\"2025-02-25\">Feb 25 Tu, 2025</option>
<option value=\"2025-02-26\">Feb 26 We, 2025</option>
<option value=\"2025-02-27\">Feb 27 Th, 2025</option>
<option value=\"2025-02-28\">Feb 28 Fr, 2025</option>
<option value=\"2025-03-01\">Mar 01 Sa, 2025</option>
<option value=\"2025-03-02\">Mar 02 Su, 2025</option>
<option value=\"2025-03-03\">Mar 03 Mo, 2025</option>
<option value=\"2025-03-04\">Mar 04 Tu, 2025</option>
<option value=\"2025-03-05\">Mar 05 We, 2025</option>
<option value=\"2025-03-06\">Mar 06 Th, 2025</option>
<option value=\"2025-03-07\">Mar 07 Fr, 2025</option>
<option value=\"2025-03-08\">Mar 08 Sa, 2025</option>
<option value=\"2025-03-09\">Mar 09 Su, 2025</option>
<option value=\"2025-03-10\">Mar 10 Mo, 2025</option>
<option value=\"2025-03-11\">Mar 11 Tu, 2025</option>
<option value=\"2025-03-12\">Mar 12 We, 2025</option>
<option value=\"2025-03-13\">Mar 13 Th, 2025</option>
<option value=\"2025-03-14\">Mar 14 Fr, 2025</option>
<option value=\"2025-03-15\">Mar 15 Sa, 2025</option>
<option value=\"2025-03-16\">Mar 16 Su, 2025</option>
<option value=\"2025-03-17\">Mar 17 Mo, 2025</option>
<option value=\"2025-03-18\">Mar 18 Tu, 2025</option>
<option value=\"2025-03-19\">Mar 19 We, 2025</option>
<option value=\"2025-03-20\">Mar 20 Th, 2025</option>
<option value=\"2025-03-21\">Mar 21 Fr, 2025</option>
<option value=\"2025-03-22\">Mar 22 Sa, 2025</option>
<option value=\"2025-03-23\">Mar 23 Su, 2025</option>
<option value=\"2025-03-24\">Mar 24 Mo, 2025</option>
<option value=\"2025-03-25\">Mar 25 Tu, 2025</option>
<option value=\"2025-03-26\">Mar 26 We, 2025</option>
<option value=\"2025-03-27\">Mar 27 Th, 2025</option>
<option value=\"2025-03-28\">Mar 28 Fr, 2025</option>
<option value=\"2025-03-29\">Mar 29 Sa, 2025</option>
<option value=\"2025-03-30\">Mar 30 Su, 2025</option>
<option value=\"2025-03-31\">Mar 31 Mo, 2025</option>
<option value=\"2025-04-01\">Apr 01 Tu, 2025</option>
<option value=\"2025-04-02\">Apr 02 We, 2025</option>
<option value=\"2025-04-03\">Apr 03 Th, 2025</option>
<option value=\"2025-04-04\">Apr 04 Fr, 2025</option>
<option value=\"2025-04-05\">Apr 05 Sa, 2025</option>
<option value=\"2025-04-06\">Apr 06 Su, 2025</option>
<option value=\"2025-04-07\">Apr 07 Mo, 2025</option>
<option value=\"2025-04-08\">Apr 08 Tu, 2025</option>
<option value=\"2025-04-09\">Apr 09 We, 2025</option>
<option value=\"2025-04-10\">Apr 10 Th, 2025</option>
<option value=\"2025-04-11\">Apr 11 Fr, 2025</option>
<option value=\"2025-04-12\">Apr 12 Sa, 2025</option>
<option value=\"2025-04-13\">Apr 13 Su, 2025</option>
<option value=\"2025-04-14\">Apr 14 Mo, 2025</option>
<option value=\"2025-04-15\">Apr 15 Tu, 2025</option>
<option value=\"2025-04-16\">Apr 16 We, 2025</option>
<option value=\"2025-04-17\">Apr 17 Th, 2025</option>
<option value=\"2025-04-18\">Apr 18 Fr, 2025</option>
<option value=\"2025-04-19\">Apr 19 Sa, 2025</option>
<option value=\"2025-04-20\">Apr 20 Su, 2025</option>
<option value=\"2025-04-21\">Apr 21 Mo, 2025</option>
<option value=\"2025-04-22\">Apr 22 Tu, 2025</option>
<option value=\"2025-04-23\">Apr 23 We, 2025</option>
<option value=\"2025-04-24\">Apr 24 Th, 2025</option>
<option value=\"2025-04-25\">Apr 25 Fr, 2025</option>
<option value=\"2025-04-26\">Apr 26 Sa, 2025</option>
<option value=\"2025-04-27\">Apr 27 Su, 2025</option>
<option value=\"2025-04-28\">Apr 28 Mo, 2025</option>
<option value=\"2025-04-29\">Apr 29 Tu, 2025</option>
<option value=\"2025-04-30\">Apr 30 We, 2025</option>
<option value=\"2025-05-01\">May 01 Th, 2025</option>
<option value=\"2025-05-02\">May 02 Fr, 2025</option>
<option value=\"2025-05-03\">May 03 Sa, 2025</option>
<option value=\"2025-05-04\">May 04 Su, 2025</option>
<option value=\"2025-05-05\">May 05 Mo, 2025</option>
<option value=\"2025-05-06\">May 06 Tu, 2025</option>
<option value=\"2025-05-07\">May 07 We, 2025</option>
<option value=\"2025-05-08\">May 08 Th, 2025</option>
<option value=\"2025-05-09\">May 09 Fr, 2025</option>
<option value=\"2025-05-10\">May 10 Sa, 2025</option>
<option value=\"2025-05-11\">May 11 Su, 2025</option>
<option value=\"2025-05-12\">May 12 Mo, 2025</option>
<option value=\"2025-05-13\">May 13 Tu, 2025</option>
<option value=\"2025-05-14\">May 14 We, 2025</option>
<option value=\"2025-05-15\">May 15 Th, 2025</option>
<option value=\"2025-05-16\">May 16 Fr, 2025</option>
<option value=\"2025-05-17\">May 17 Sa, 2025</option>
<option value=\"2025-05-18\">May 18 Su, 2025</option>
<option value=\"2025-05-19\">May 19 Mo, 2025</option>
<option value=\"2025-05-20\">May 20 Tu, 2025</option>
<option value=\"2025-05-21\">May 21 We, 2025</option>
<option value=\"2025-05-22\">May 22 Th, 2025</option>
<option value=\"2025-05-23\">May 23 Fr, 2025</option>
<option value=\"2025-05-24\">May 24 Sa, 2025</option>
<option value=\"2025-05-25\">May 25 Su, 2025</option>
<option value=\"2025-05-26\">May 26 Mo, 2025</option>
<option value=\"2025-05-27\">May 27 Tu, 2025</option>
<option value=\"2025-05-28\">May 28 We, 2025</option>
<option value=\"2025-05-29\">May 29 Th, 2025</option>
<option value=\"2025-05-30\">May 30 Fr, 2025</option>
<option value=\"2025-05-31\">May 31 Sa, 2025</option>
<option value=\"2025-06-01\">Jun 01 Su, 2025</option>
<option value=\"2025-06-02\">Jun 02 Mo, 2025</option>
<option value=\"2025-06-03\">Jun 03 Tu, 2025</option>
<option value=\"2025-06-04\">Jun 04 We, 2025</option>
<option value=\"2025-06-05\">Jun 05 Th, 2025</option>
<option value=\"2025-06-06\">Jun 06 Fr, 2025</option>
<option value=\"2025-06-07\">Jun 07 Sa, 2025</option>
<option value=\"2025-06-08\">Jun 08 Su, 2025</option>
<option value=\"2025-06-09\">Jun 09 Mo, 2025</option>
<option value=\"2025-06-10\">Jun 10 Tu, 2025</option>
<option value=\"2025-06-11\">Jun 11 We, 2025</option>
<option value=\"2025-06-12\">Jun 12 Th, 2025</option>
<option value=\"2025-06-13\">Jun 13 Fr, 2025</option>
<option value=\"2025-06-14\">Jun 14 Sa, 2025</option>
<option value=\"2025-06-15\">Jun 15 Su, 2025</option>
<option value=\"2025-06-16\">Jun 16 Mo, 2025</option>
<option value=\"2025-06-17\">Jun 17 Tu, 2025</option>
<option value=\"2025-06-18\">Jun 18 We, 2025</option>
<option value=\"2025-06-19\">Jun 19 Th, 2025</option>
<option value=\"2025-06-20\">Jun 20 Fr, 2025</option>
<option value=\"2025-06-21\">Jun 21 Sa, 2025</option>
<option value=\"2025-06-22\">Jun 22 Su, 2025</option>
<option value=\"2025-06-23\">Jun 23 Mo, 2025</option>
<option value=\"2025-06-24\">Jun 24 Tu, 2025</option>
<option value=\"2025-06-25\">Jun 25 We, 2025</option>
<option value=\"2025-06-26\">Jun 26 Th, 2025</option>
<option value=\"2025-06-27\">Jun 27 Fr, 2025</option>
<option value=\"2025-06-28\">Jun 28 Sa, 2025</option>
<option value=\"2025-06-29\">Jun 29 Su, 2025</option>
<option value=\"2025-06-30\">Jun 30 Mo, 2025</option>
<option value=\"2025-07-01\">Jul 01 Tu, 2025</option>
<option value=\"2025-07-02\">Jul 02 We, 2025</option>
<option value=\"2025-07-03\">Jul 03 Th, 2025</option>
<option value=\"2025-07-04\">Jul 04 Fr, 2025</option>
<option value=\"2025-07-05\">Jul 05 Sa, 2025</option>
<option value=\"2025-07-06\">Jul 06 Su, 2025</option>
<option value=\"2025-07-07\">Jul 07 Mo, 2025</option>
<option value=\"2025-07-08\">Jul 08 Tu, 2025</option>
<option value=\"2025-07-09\">Jul 09 We, 2025</option>
<option value=\"2025-07-10\">Jul 10 Th, 2025</option>
<option value=\"2025-07-11\">Jul 11 Fr, 2025</option>
<option value=\"2025-07-12\">Jul 12 Sa, 2025</option>
<option value=\"2025-07-13\">Jul 13 Su, 2025</option>
<option value=\"2025-07-14\">Jul 14 Mo, 2025</option>
<option value=\"2025-07-15\">Jul 15 Tu, 2025</option>
<option value=\"2025-07-16\">Jul 16 We, 2025</option>
<option value=\"2025-07-17\">Jul 17 Th, 2025</option>
<option value=\"2025-07-18\">Jul 18 Fr, 2025</option>
<option value=\"2025-07-19\">Jul 19 Sa, 2025</option>
<option value=\"2025-07-20\">Jul 20 Su, 2025</option>
<option value=\"2025-07-21\">Jul 21 Mo, 2025</option>
<option value=\"2025-07-22\">Jul 22 Tu, 2025</option>
<option value=\"2025-07-23\">Jul 23 We, 2025</option>
<option value=\"2025-07-24\">Jul 24 Th, 2025</option>
<option value=\"2025-07-25\">Jul 25 Fr, 2025</option>
<option value=\"2025-07-26\">Jul 26 Sa, 2025</option>
<option value=\"2025-07-27\">Jul 27 Su, 2025</option>
<option value=\"2025-07-28\">Jul 28 Mo, 2025</option>
<option value=\"2025-07-29\">Jul 29 Tu, 2025</option>
<option value=\"2025-07-30\">Jul 30 We, 2025</option>
<option value=\"2025-07-31\">Jul 31 Th, 2025</option>
<option value=\"2025-08-01\">Aug 01 Fr, 2025</option>
<option value=\"2025-08-02\">Aug 02 Sa, 2025</option>
<option value=\"2025-08-03\">Aug 03 Su, 2025</option>
<option value=\"2025-08-04\">Aug 04 Mo, 2025</option>
<option value=\"2025-08-05\">Aug 05 Tu, 2025</option>
<option value=\"2025-08-06\">Aug 06 We, 2025</option>
<option value=\"2025-08-07\">Aug 07 Th, 2025</option>
<option value=\"2025-08-08\">Aug 08 Fr, 2025</option>
<option value=\"2025-08-09\">Aug 09 Sa, 2025</option>
<option value=\"2025-08-10\">Aug 10 Su, 2025</option>
<option value=\"2025-08-11\">Aug 11 Mo, 2025</option>
<option value=\"2025-08-12\">Aug 12 Tu, 2025</option>
<option value=\"2025-08-13\">Aug 13 We, 2025</option>
<option value=\"2025-08-14\">Aug 14 Th, 2025</option>
<option value=\"2025-08-15\">Aug 15 Fr, 2025</option>
<option value=\"2025-08-16\">Aug 16 Sa, 2025</option>
<option value=\"2025-08-17\">Aug 17 Su, 2025</option>
<option value=\"2025-08-18\">Aug 18 Mo, 2025</option>
<option value=\"2025-08-19\">Aug 19 Tu, 2025</option>
<option value=\"2025-08-20\">Aug 20 We, 2025</option>
<option value=\"2025-08-21\">Aug 21 Th, 2025</option>
<option value=\"2025-08-22\">Aug 22 Fr, 2025</option>
<option value=\"2025-08-23\">Aug 23 Sa, 2025</option>
<option value=\"2025-08-24\">Aug 24 Su, 2025</option>
<option value=\"2025-08-25\">Aug 25 Mo, 2025</option>
<option value=\"2025-08-26\">Aug 26 Tu, 2025</option>
<option value=\"2025-08-27\">Aug 27 We, 2025</option>
<option value=\"2025-08-28\">Aug 28 Th, 2025</option>
<option value=\"2025-08-29\">Aug 29 Fr, 2025</option>
<option value=\"2025-08-30\">Aug 30 Sa, 2025</option>
<option value=\"2025-08-31\">Aug 31 Su, 2025</option>
<option value=\"2025-09-01\">Sep 01 Mo, 2025</option>
<option value=\"2025-09-02\">Sep 02 Tu, 2025</option>
<option value=\"2025-09-03\">Sep 03 We, 2025</option>
<option value=\"2025-09-04\">Sep 04 Th, 2025</option>
<option value=\"2025-09-05\">Sep 05 Fr, 2025</option>
<option value=\"2025-09-06\">Sep 06 Sa, 2025</option>
<option value=\"2025-09-07\">Sep 07 Su, 2025</option>
<option value=\"2025-09-08\">Sep 08 Mo, 2025</option>
<option value=\"2025-09-09\">Sep 09 Tu, 2025</option>
<option value=\"2025-09-10\">Sep 10 We, 2025</option>
<option value=\"2025-09-11\">Sep 11 Th, 2025</option>
<option value=\"2025-09-12\">Sep 12 Fr, 2025</option>
<option value=\"2025-09-13\">Sep 13 Sa, 2025</option>
<option value=\"2025-09-14\">Sep 14 Su, 2025</option>
<option value=\"2025-09-15\">Sep 15 Mo, 2025</option>
<option value=\"2025-09-16\">Sep 16 Tu, 2025</option>
<option value=\"2025-09-17\">Sep 17 We, 2025</option>
<option value=\"2025-09-18\">Sep 18 Th, 2025</option>
<option value=\"2025-09-19\">Sep 19 Fr, 2025</option>
<option value=\"2025-09-20\">Sep 20 Sa, 2025</option>
<option value=\"2025-09-21\">Sep 21 Su, 2025</option>
<option value=\"2025-09-22\">Sep 22 Mo, 2025</option>
<option value=\"2025-09-23\">Sep 23 Tu, 2025</option>
<option value=\"2025-09-24\">Sep 24 We, 2025</option>
<option value=\"2025-09-25\">Sep 25 Th, 2025</option>
<option value=\"2025-09-26\">Sep 26 Fr, 2025</option>
<option value=\"2025-09-27\">Sep 27 Sa, 2025</option>
<option value=\"2025-09-28\">Sep 28 Su, 2025</option>
<option value=\"2025-09-29\">Sep 29 Mo, 2025</option>
<option value=\"2025-09-30\">Sep 30 Tu, 2025</option>
<option value=\"2025-10-01\">Oct 01 We, 2025</option>
<option value=\"2025-10-02\">Oct 02 Th, 2025</option>
<option value=\"2025-10-03\">Oct 03 Fr, 2025</option>
<option value=\"2025-10-04\">Oct 04 Sa, 2025</option>
<option value=\"2025-10-05\">Oct 05 Su, 2025</option>
<option value=\"2025-10-06\">Oct 06 Mo, 2025</option>
<option value=\"2025-10-07\">Oct 07 Tu, 2025</option>
<option value=\"2025-10-08\">Oct 08 We, 2025</option>
<option value=\"2025-10-09\">Oct 09 Th, 2025</option>
<option value=\"2025-10-10\">Oct 10 Fr, 2025</option>
<option value=\"2025-10-11\">Oct 11 Sa, 2025</option>
<option value=\"2025-10-12\">Oct 12 Su, 2025</option>
<option value=\"2025-10-13\">Oct 13 Mo, 2025</option>
<option value=\"2025-10-14\">Oct 14 Tu, 2025</option>
<option value=\"2025-10-15\">Oct 15 We, 2025</option>
<option value=\"2025-10-16\">Oct 16 Th, 2025</option>
<option value=\"2025-10-17\">Oct 17 Fr, 2025</option>
<option value=\"2025-10-18\">Oct 18 Sa, 2025</option>
<option value=\"2025-10-19\">Oct 19 Su, 2025</option>
<option value=\"2025-10-20\">Oct 20 Mo, 2025</option>
<option value=\"2025-10-21\">Oct 21 Tu, 2025</option>
<option value=\"2025-10-22\">Oct 22 We, 2025</option>
<option value=\"2025-10-23\">Oct 23 Th, 2025</option>
<option value=\"2025-10-24\">Oct 24 Fr, 2025</option>
<option value=\"2025-10-25\">Oct 25 Sa, 2025</option>
<option value=\"2025-10-26\">Oct 26 Su, 2025</option>
<option value=\"2025-10-27\">Oct 27 Mo, 2025</option>
<option value=\"2025-10-28\">Oct 28 Tu, 2025</option>
<option value=\"2025-10-29\">Oct 29 We, 2025</option>
<option value=\"2025-10-30\">Oct 30 Th, 2025</option>
<option value=\"2025-10-31\">Oct 31 Fr, 2025</option>
<option value=\"2025-11-01\">Nov 01 Sa, 2025</option>
<option value=\"2025-11-02\">Nov 02 Su, 2025</option>
<option value=\"2025-11-03\">Nov 03 Mo, 2025</option>
<option value=\"2025-11-04\">Nov 04 Tu, 2025</option>
<option value=\"2025-11-05\">Nov 05 We, 2025</option>
<option value=\"2025-11-06\">Nov 06 Th, 2025</option>
<option value=\"2025-11-07\">Nov 07 Fr, 2025</option>
<option value=\"2025-11-08\">Nov 08 Sa, 2025</option>
<option value=\"2025-11-09\">Nov 09 Su, 2025</option>
<option value=\"2025-11-10\">Nov 10 Mo, 2025</option>
<option value=\"2025-11-11\">Nov 11 Tu, 2025</option>
<option value=\"2025-11-12\">Nov 12 We, 2025</option>
<option value=\"2025-11-13\">Nov 13 Th, 2025</option>
<option value=\"2025-11-14\">Nov 14 Fr, 2025</option>
<option value=\"2025-11-15\">Nov 15 Sa, 2025</option>
<option value=\"2025-11-16\">Nov 16 Su, 2025</option>
<option value=\"2025-11-17\">Nov 17 Mo, 2025</option>
<option value=\"2025-11-18\">Nov 18 Tu, 2025</option>
<option value=\"2025-11-19\">Nov 19 We, 2025</option>
<option value=\"2025-11-20\">Nov 20 Th, 2025</option>
<option value=\"2025-11-21\">Nov 21 Fr, 2025</option>
<option value=\"2025-11-22\">Nov 22 Sa, 2025</option>
<option value=\"2025-11-23\">Nov 23 Su, 2025</option>
<option value=\"2025-11-24\">Nov 24 Mo, 2025</option>
<option value=\"2025-11-25\">Nov 25 Tu, 2025</option>
<option value=\"2025-11-26\">Nov 26 We, 2025</option>
<option value=\"2025-11-27\">Nov 27 Th, 2025</option>
<option value=\"2025-11-28\">Nov 28 Fr, 2025</option>
<option value=\"2025-11-29\">Nov 29 Sa, 2025</option>
<option value=\"2025-11-30\">Nov 30 Su, 2025</option>
<option value=\"2025-12-01\">Dec 01 Mo, 2025</option>
<option value=\"2025-12-02\">Dec 02 Tu, 2025</option>
<option value=\"2025-12-03\">Dec 03 We, 2025</option>
<option value=\"2025-12-04\">Dec 04 Th, 2025</option>
<option value=\"2025-12-05\">Dec 05 Fr, 2025</option>
<option value=\"2025-12-06\">Dec 06 Sa, 2025</option>
<option value=\"2025-12-07\">Dec 07 Su, 2025</option>
<option value=\"2025-12-08\">Dec 08 Mo, 2025</option>
<option value=\"2025-12-09\">Dec 09 Tu, 2025</option>
<option value=\"2025-12-10\">Dec 10 We, 2025</option>
<option value=\"2025-12-11\">Dec 11 Th, 2025</option>
<option value=\"2025-12-12\">Dec 12 Fr, 2025</option>
<option value=\"2025-12-13\">Dec 13 Sa, 2025</option>
<option value=\"2025-12-14\">Dec 14 Su, 2025</option>
<option value=\"2025-12-15\">Dec 15 Mo, 2025</option>
<option value=\"2025-12-16\">Dec 16 Tu, 2025</option>
<option value=\"2025-12-17\">Dec 17 We, 2025</option>
<option value=\"2025-12-18\">Dec 18 Th, 2025</option>
<option value=\"2025-12-19\">Dec 19 Fr, 2025</option>
<option value=\"2025-12-20\">Dec 20 Sa, 2025</option>
<option value=\"2025-12-21\">Dec 21 Su, 2025</option>
<option value=\"2025-12-22\">Dec 22 Mo, 2025</option>
<option value=\"2025-12-23\">Dec 23 Tu, 2025</option>
<option value=\"2025-12-24\">Dec 24 We, 2025</option>
<option value=\"2025-12-25\">Dec 25 Th, 2025</option>
<option value=\"2025-12-26\">Dec 26 Fr, 2025</option>
<option value=\"2025-12-27\">Dec 27 Sa, 2025</option>
<option value=\"2025-12-28\">Dec 28 Su, 2025</option>
<option value=\"2025-12-29\">Dec 29 Mo, 2025</option>
<option value=\"2025-12-30\">Dec 30 Tu, 2025</option>
<option value=\"2025-12-31\">Dec 31 We, 2025</option>
<option value=\"2026-01-01\">Jan 01 Th, 2026</option>
<option value=\"2026-01-02\">Jan 02 Fr, 2026</option>
<option value=\"2026-01-03\">Jan 03 Sa, 2026</option>
<option value=\"2026-01-04\">Jan 04 Su, 2026</option>
<option value=\"2026-01-05\">Jan 05 Mo, 2026</option>
<option value=\"2026-01-06\">Jan 06 Tu, 2026</option>
<option value=\"2026-01-07\">Jan 07 We, 2026</option>
<option value=\"2026-01-08\">Jan 08 Th, 2026</option>
<option value=\"2026-01-09\">Jan 09 Fr, 2026</option>
<option value=\"2026-01-10\">Jan 10 Sa, 2026</option>
<option value=\"2026-01-11\">Jan 11 Su, 2026</option>
<option value=\"2026-01-12\">Jan 12 Mo, 2026</option>
<option value=\"2026-01-13\">Jan 13 Tu, 2026</option>
<option value=\"2026-01-14\">Jan 14 We, 2026</option>
<option value=\"2026-01-15\">Jan 15 Th, 2026</option>
<option value=\"2026-01-16\">Jan 16 Fr, 2026</option>
<option value=\"2026-01-17\">Jan 17 Sa, 2026</option>
<option value=\"2026-01-18\">Jan 18 Su, 2026</option>
<option value=\"2026-01-19\">Jan 19 Mo, 2026</option>
<option value=\"2026-01-20\">Jan 20 Tu, 2026</option>
<option value=\"2026-01-21\">Jan 21 We, 2026</option>
<option value=\"2026-01-22\">Jan 22 Th, 2026</option>
<option value=\"2026-01-23\">Jan 23 Fr, 2026</option>
<option value=\"2026-01-24\">Jan 24 Sa, 2026</option>
<option value=\"2026-01-25\">Jan 25 Su, 2026</option>
<option value=\"2026-01-26\">Jan 26 Mo, 2026</option>
<option value=\"2026-01-27\">Jan 27 Tu, 2026</option>
<option value=\"2026-01-28\">Jan 28 We, 2026</option>
<option value=\"2026-01-29\">Jan 29 Th, 2026</option>
<option value=\"2026-01-30\">Jan 30 Fr, 2026</option>
<option value=\"2026-01-31\">Jan 31 Sa, 2026</option>
<option value=\"2026-02-01\">Feb 01 Su, 2026</option>
<option value=\"2026-02-02\">Feb 02 Mo, 2026</option>
<option value=\"2026-02-03\">Feb 03 Tu, 2026</option>
<option value=\"2026-02-04\">Feb 04 We, 2026</option>
<option value=\"2026-02-05\">Feb 05 Th, 2026</option>
<option value=\"2026-02-06\">Feb 06 Fr, 2026</option>
<option value=\"2026-02-07\">Feb 07 Sa, 2026</option>
<option value=\"2026-02-08\">Feb 08 Su, 2026</option>
<option value=\"2026-02-09\">Feb 09 Mo, 2026</option>
<option value=\"2026-02-10\">Feb 10 Tu, 2026</option>
<option value=\"2026-02-11\">Feb 11 We, 2026</option>
<option value=\"2026-02-12\">Feb 12 Th, 2026</option>
<option value=\"2026-02-13\">Feb 13 Fr, 2026</option>
<option value=\"2026-02-14\">Feb 14 Sa, 2026</option>
<option value=\"2026-02-15\">Feb 15 Su, 2026</option>
<option value=\"2026-02-16\">Feb 16 Mo, 2026</option>
<option value=\"2026-02-17\">Feb 17 Tu, 2026</option>
<option value=\"2026-02-18\">Feb 18 We, 2026</option>
<option value=\"2026-02-19\">Feb 19 Th, 2026</option>
<option value=\"2026-02-20\">Feb 20 Fr, 2026</option>
<option value=\"2026-02-21\">Feb 21 Sa, 2026</option>
<option value=\"2026-02-22\">Feb 22 Su, 2026</option>
<option value=\"2026-02-23\">Feb 23 Mo, 2026</option>
<option value=\"2026-02-24\">Feb 24 Tu, 2026</option>
<option value=\"2026-02-25\">Feb 25 We, 2026</option>
<option value=\"2026-02-26\">Feb 26 Th, 2026</option>
<option value=\"2026-02-27\">Feb 27 Fr, 2026</option>
<option value=\"2026-02-28\">Feb 28 Sa, 2026</option>
<option value=\"2026-03-01\">Mar 01 Su, 2026</option>
<option value=\"2026-03-02\">Mar 02 Mo, 2026</option>
<option value=\"2026-03-03\">Mar 03 Tu, 2026</option>
<option value=\"2026-03-04\">Mar 04 We, 2026</option>
<option value=\"2026-03-05\">Mar 05 Th, 2026</option>
<option value=\"2026-03-06\">Mar 06 Fr, 2026</option>
<option value=\"2026-03-07\">Mar 07 Sa, 2026</option>
<option value=\"2026-03-08\">Mar 08 Su, 2026</option>
<option value=\"2026-03-09\">Mar 09 Mo, 2026</option>
<option value=\"2026-03-10\">Mar 10 Tu, 2026</option>
<option value=\"2026-03-11\">Mar 11 We, 2026</option>
<option value=\"2026-03-12\">Mar 12 Th, 2026</option>
<option value=\"2026-03-13\">Mar 13 Fr, 2026</option>
<option value=\"2026-03-14\">Mar 14 Sa, 2026</option>
<option value=\"2026-03-15\">Mar 15 Su, 2026</option>
<option value=\"2026-03-16\">Mar 16 Mo, 2026</option>
<option value=\"2026-03-17\">Mar 17 Tu, 2026</option>
<option value=\"2026-03-18\">Mar 18 We, 2026</option>
<option value=\"2026-03-19\">Mar 19 Th, 2026</option>
<option value=\"2026-03-20\">Mar 20 Fr, 2026</option>
<option value=\"2026-03-21\">Mar 21 Sa, 2026</option>
<option value=\"2026-03-22\">Mar 22 Su, 2026</option>
<option value=\"2026-03-23\">Mar 23 Mo, 2026</option>
<option value=\"2026-03-24\">Mar 24 Tu, 2026</option>
<option value=\"2026-03-25\">Mar 25 We, 2026</option>
<option value=\"2026-03-26\">Mar 26 Th, 2026</option>
<option value=\"2026-03-27\">Mar 27 Fr, 2026</option>
<option value=\"2026-03-28\">Mar 28 Sa, 2026</option>
<option value=\"2026-03-29\">Mar 29 Su, 2026</option>
<option value=\"2026-03-30\">Mar 30 Mo, 2026</option>
<option value=\"2026-03-31\">Mar 31 Tu, 2026</option>
<option value=\"2026-04-01\">Apr 01 We, 2026</option>
<option value=\"2026-04-02\">Apr 02 Th, 2026</option>
<option value=\"2026-04-03\">Apr 03 Fr, 2026</option>
<option value=\"2026-04-04\">Apr 04 Sa, 2026</option>
<option value=\"2026-04-05\">Apr 05 Su, 2026</option>
<option value=\"2026-04-06\">Apr 06 Mo, 2026</option>
<option value=\"2026-04-07\">Apr 07 Tu, 2026</option>
<option value=\"2026-04-08\">Apr 08 We, 2026</option>
<option value=\"2026-04-09\">Apr 09 Th, 2026</option>
<option value=\"2026-04-10\">Apr 10 Fr, 2026</option>
<option value=\"2026-04-11\">Apr 11 Sa, 2026</option>
<option value=\"2026-04-12\">Apr 12 Su, 2026</option>
<option value=\"2026-04-13\">Apr 13 Mo, 2026</option>
<option value=\"2026-04-14\">Apr 14 Tu, 2026</option>
<option value=\"2026-04-15\">Apr 15 We, 2026</option>
<option value=\"2026-04-16\">Apr 16 Th, 2026</option>
<option value=\"2026-04-17\">Apr 17 Fr, 2026</option>
<option value=\"2026-04-18\">Apr 18 Sa, 2026</option>
<option value=\"2026-04-19\">Apr 19 Su, 2026</option>
<option value=\"2026-04-20\">Apr 20 Mo, 2026</option>
<option value=\"2026-04-21\">Apr 21 Tu, 2026</option>
<option value=\"2026-04-22\">Apr 22 We, 2026</option>
<option value=\"2026-04-23\">Apr 23 Th, 2026</option>
<option value=\"2026-04-24\">Apr 24 Fr, 2026</option>
<option value=\"2026-04-25\">Apr 25 Sa, 2026</option>
<option value=\"2026-04-26\">Apr 26 Su, 2026</option>
<option value=\"2026-04-27\">Apr 27 Mo, 2026</option>
<option value=\"2026-04-28\">Apr 28 Tu, 2026</option>
<option value=\"2026-04-29\">Apr 29 We, 2026</option>
<option value=\"2026-04-30\">Apr 30 Th, 2026</option>
<option value=\"2026-05-01\">May 01 Fr, 2026</option>
<option value=\"2026-05-02\">May 02 Sa, 2026</option>
<option value=\"2026-05-03\">May 03 Su, 2026</option>
<option value=\"2026-05-04\">May 04 Mo, 2026</option>
<option value=\"2026-05-05\">May 05 Tu, 2026</option>
<option value=\"2026-05-06\">May 06 We, 2026</option>
<option value=\"2026-05-07\">May 07 Th, 2026</option>
<option value=\"2026-05-08\">May 08 Fr, 2026</option>
<option value=\"2026-05-09\">May 09 Sa, 2026</option>
<option value=\"2026-05-10\">May 10 Su, 2026</option>
<option value=\"2026-05-11\">May 11 Mo, 2026</option>
<option value=\"2026-05-12\">May 12 Tu, 2026</option>
<option value=\"2026-05-13\">May 13 We, 2026</option>
<option value=\"2026-05-14\">May 14 Th, 2026</option>
<option value=\"2026-05-15\">May 15 Fr, 2026</option>
<option value=\"2026-05-16\">May 16 Sa, 2026</option>
<option value=\"2026-05-17\">May 17 Su, 2026</option>
<option value=\"2026-05-18\">May 18 Mo, 2026</option>
<option value=\"2026-05-19\">May 19 Tu, 2026</option>
<option value=\"2026-05-20\">May 20 We, 2026</option>
<option value=\"2026-05-21\">May 21 Th, 2026</option>
<option value=\"2026-05-22\">May 22 Fr, 2026</option>
<option value=\"2026-05-23\">May 23 Sa, 2026</option>
<option value=\"2026-05-24\">May 24 Su, 2026</option>
<option value=\"2026-05-25\">May 25 Mo, 2026</option>
<option value=\"2026-05-26\">May 26 Tu, 2026</option>
<option value=\"2026-05-27\">May 27 We, 2026</option>
<option value=\"2026-05-28\">May 28 Th, 2026</option>
<option value=\"2026-05-29\">May 29 Fr, 2026</option>
<option value=\"2026-05-30\">May 30 Sa, 2026</option>
<option value=\"2026-05-31\">May 31 Su, 2026</option>
<option value=\"2026-06-01\">Jun 01 Mo, 2026</option>
<option value=\"2026-06-02\">Jun 02 Tu, 2026</option>
<option value=\"2026-06-03\">Jun 03 We, 2026</option>
<option value=\"2026-06-04\">Jun 04 Th, 2026</option>
<option value=\"2026-06-05\">Jun 05 Fr, 2026</option>
<option value=\"2026-06-06\">Jun 06 Sa, 2026</option>
<option value=\"2026-06-07\">Jun 07 Su, 2026</option>
<option value=\"2026-06-08\">Jun 08 Mo, 2026</option>
<option value=\"2026-06-09\">Jun 09 Tu, 2026</option>
<option value=\"2026-06-10\">Jun 10 We, 2026</option>
<option value=\"2026-06-11\">Jun 11 Th, 2026</option>
<option value=\"2026-06-12\">Jun 12 Fr, 2026</option>
<option value=\"2026-06-13\">Jun 13 Sa, 2026</option>
<option value=\"2026-06-14\">Jun 14 Su, 2026</option>
<option value=\"2026-06-15\">Jun 15 Mo, 2026</option>
<option value=\"2026-06-16\">Jun 16 Tu, 2026</option>
<option value=\"2026-06-17\">Jun 17 We, 2026</option>
<option value=\"2026-06-18\">Jun 18 Th, 2026</option>
<option value=\"2026-06-19\">Jun 19 Fr, 2026</option>
<option value=\"2026-06-20\">Jun 20 Sa, 2026</option>
<option value=\"2026-06-21\">Jun 21 Su, 2026</option>
<option value=\"2026-06-22\">Jun 22 Mo, 2026</option>
<option value=\"2026-06-23\">Jun 23 Tu, 2026</option>
<option value=\"2026-06-24\">Jun 24 We, 2026</option>
<option value=\"2026-06-25\">Jun 25 Th, 2026</option>
<option value=\"2026-06-26\">Jun 26 Fr, 2026</option>
<option value=\"2026-06-27\">Jun 27 Sa, 2026</option>
<option value=\"2026-06-28\">Jun 28 Su, 2026</option>
<option value=\"2026-06-29\">Jun 29 Mo, 2026</option>
<option value=\"2026-06-30\">Jun 30 Tu, 2026</option>
<option value=\"2026-07-01\">Jul 01 We, 2026</option>
<option value=\"2026-07-02\">Jul 02 Th, 2026</option>
<option value=\"2026-07-03\">Jul 03 Fr, 2026</option>
<option value=\"2026-07-04\">Jul 04 Sa, 2026</option>
<option value=\"2026-07-05\">Jul 05 Su, 2026</option>
<option value=\"2026-07-06\">Jul 06 Mo, 2026</option>
<option value=\"2026-07-07\">Jul 07 Tu, 2026</option>
<option value=\"2026-07-08\">Jul 08 We, 2026</option>
<option value=\"2026-07-09\">Jul 09 Th, 2026</option>
<option value=\"2026-07-10\">Jul 10 Fr, 2026</option>
<option value=\"2026-07-11\">Jul 11 Sa, 2026</option>
<option value=\"2026-07-12\">Jul 12 Su, 2026</option>
<option value=\"2026-07-13\">Jul 13 Mo, 2026</option>
<option value=\"2026-07-14\">Jul 14 Tu, 2026</option>
<option value=\"2026-07-15\">Jul 15 We, 2026</option>
<option value=\"2026-07-16\">Jul 16 Th, 2026</option>
<option value=\"2026-07-17\">Jul 17 Fr, 2026</option>
<option value=\"2026-07-18\">Jul 18 Sa, 2026</option>
<option value=\"2026-07-19\">Jul 19 Su, 2026</option>
<option value=\"2026-07-20\">Jul 20 Mo, 2026</option>
<option value=\"2026-07-21\">Jul 21 Tu, 2026</option>
<option value=\"2026-07-22\">Jul 22 We, 2026</option>
<option value=\"2026-07-23\">Jul 23 Th, 2026</option>
<option value=\"2026-07-24\">Jul 24 Fr, 2026</option>
<option value=\"2026-07-25\">Jul 25 Sa, 2026</option>
<option value=\"2026-07-26\">Jul 26 Su, 2026</option>
<option value=\"2026-07-27\">Jul 27 Mo, 2026</option>
<option value=\"2026-07-28\">Jul 28 Tu, 2026</option>
<option value=\"2026-07-29\">Jul 29 We, 2026</option>
<option value=\"2026-07-30\">Jul 30 Th, 2026</option>
<option value=\"2026-07-31\">Jul 31 Fr, 2026</option>
<option value=\"2026-08-01\">Aug 01 Sa, 2026</option>
<option value=\"2026-08-02\">Aug 02 Su, 2026</option>
<option value=\"2026-08-03\">Aug 03 Mo, 2026</option>
<option value=\"2026-08-04\">Aug 04 Tu, 2026</option>
<option value=\"2026-08-05\">Aug 05 We, 2026</option>
<option value=\"2026-08-06\">Aug 06 Th, 2026</option>
<option value=\"2026-08-07\">Aug 07 Fr, 2026</option>
<option value=\"2026-08-08\">Aug 08 Sa, 2026</option>
<option value=\"2026-08-09\">Aug 09 Su, 2026</option>
<option value=\"2026-08-10\">Aug 10 Mo, 2026</option>
<option value=\"2026-08-11\">Aug 11 Tu, 2026</option>
<option value=\"2026-08-12\">Aug 12 We, 2026</option>
<option value=\"2026-08-13\">Aug 13 Th, 2026</option>
<option value=\"2026-08-14\">Aug 14 Fr, 2026</option>
<option value=\"2026-08-15\">Aug 15 Sa, 2026</option>
<option value=\"2026-08-16\">Aug 16 Su, 2026</option>
<option value=\"2026-08-17\">Aug 17 Mo, 2026</option>
<option value=\"2026-08-18\">Aug 18 Tu, 2026</option>
<option value=\"2026-08-19\">Aug 19 We, 2026</option>
<option value=\"2026-08-20\">Aug 20 Th, 2026</option>
<option value=\"2026-08-21\">Aug 21 Fr, 2026</option>
<option value=\"2026-08-22\">Aug 22 Sa, 2026</option>
<option value=\"2026-08-23\">Aug 23 Su, 2026</option>
<option value=\"2026-08-24\">Aug 24 Mo, 2026</option>
<option value=\"2026-08-25\">Aug 25 Tu, 2026</option>
<option value=\"2026-08-26\">Aug 26 We, 2026</option>
<option value=\"2026-08-27\">Aug 27 Th, 2026</option>
<option value=\"2026-08-28\">Aug 28 Fr, 2026</option>
<option value=\"2026-08-29\">Aug 29 Sa, 2026</option>
<option value=\"2026-08-30\">Aug 30 Su, 2026</option>
<option value=\"2026-08-31\">Aug 31 Mo, 2026</option>
<option value=\"2026-09-01\">Sep 01 Tu, 2026</option>
<option value=\"2026-09-02\">Sep 02 We, 2026</option>
<option value=\"2026-09-03\">Sep 03 Th, 2026</option>
<option value=\"2026-09-04\">Sep 04 Fr, 2026</option>
<option value=\"2026-09-05\">Sep 05 Sa, 2026</option>
<option value=\"2026-09-06\">Sep 06 Su, 2026</option>
<option value=\"2026-09-07\">Sep 07 Mo, 2026</option>
<option value=\"2026-09-08\">Sep 08 Tu, 2026</option>
<option value=\"2026-09-09\">Sep 09 We, 2026</option>
<option value=\"2026-09-10\">Sep 10 Th, 2026</option>
<option value=\"2026-09-11\">Sep 11 Fr, 2026</option>
<option value=\"2026-09-12\">Sep 12 Sa, 2026</option>
<option value=\"2026-09-13\">Sep 13 Su, 2026</option>
<option value=\"2026-09-14\">Sep 14 Mo, 2026</option>
<option value=\"2026-09-15\">Sep 15 Tu, 2026</option>
<option value=\"2026-09-16\">Sep 16 We, 2026</option>
<option value=\"2026-09-17\">Sep 17 Th, 2026</option>
<option value=\"2026-09-18\">Sep 18 Fr, 2026</option>
<option value=\"2026-09-19\">Sep 19 Sa, 2026</option>
<option value=\"2026-09-20\">Sep 20 Su, 2026</option>
<option value=\"2026-09-21\">Sep 21 Mo, 2026</option>
<option value=\"2026-09-22\">Sep 22 Tu, 2026</option>
<option value=\"2026-09-23\">Sep 23 We, 2026</option>
<option value=\"2026-09-24\">Sep 24 Th, 2026</option>
<option value=\"2026-09-25\">Sep 25 Fr, 2026</option>
<option value=\"2026-09-26\">Sep 26 Sa, 2026</option>
<option value=\"2026-09-27\">Sep 27 Su, 2026</option>
<option value=\"2026-09-28\">Sep 28 Mo, 2026</option>
<option value=\"2026-09-29\">Sep 29 Tu, 2026</option>
<option value=\"2026-09-30\">Sep 30 We, 2026</option>
<option value=\"2026-10-01\">Oct 01 Th, 2026</option>
<option value=\"2026-10-02\">Oct 02 Fr, 2026</option>
<option value=\"2026-10-03\">Oct 03 Sa, 2026</option>
<option value=\"2026-10-04\">Oct 04 Su, 2026</option>
<option value=\"2026-10-05\">Oct 05 Mo, 2026</option>
<option value=\"2026-10-06\">Oct 06 Tu, 2026</option>
<option value=\"2026-10-07\">Oct 07 We, 2026</option>
<option value=\"2026-10-08\">Oct 08 Th, 2026</option>
<option value=\"2026-10-09\">Oct 09 Fr, 2026</option>
<option value=\"2026-10-10\">Oct 10 Sa, 2026</option>
<option value=\"2026-10-11\">Oct 11 Su, 2026</option>
<option value=\"2026-10-12\">Oct 12 Mo, 2026</option>
<option value=\"2026-10-13\">Oct 13 Tu, 2026</option>
<option value=\"2026-10-14\">Oct 14 We, 2026</option>
<option value=\"2026-10-15\">Oct 15 Th, 2026</option>
<option value=\"2026-10-16\">Oct 16 Fr, 2026</option>
<option value=\"2026-10-17\">Oct 17 Sa, 2026</option>
<option value=\"2026-10-18\">Oct 18 Su, 2026</option>
<option value=\"2026-10-19\">Oct 19 Mo, 2026</option>
<option value=\"2026-10-20\">Oct 20 Tu, 2026</option>
<option value=\"2026-10-21\">Oct 21 We, 2026</option>
<option value=\"2026-10-22\">Oct 22 Th, 2026</option>
<option value=\"2026-10-23\">Oct 23 Fr, 2026</option>
<option value=\"2026-10-24\">Oct 24 Sa, 2026</option>
<option value=\"2026-10-25\">Oct 25 Su, 2026</option>
<option value=\"2026-10-26\">Oct 26 Mo, 2026</option>
<option value=\"2026-10-27\">Oct 27 Tu, 2026</option>
<option value=\"2026-10-28\">Oct 28 We, 2026</option>
<option value=\"2026-10-29\">Oct 29 Th, 2026</option>
<option value=\"2026-10-30\">Oct 30 Fr, 2026</option>
<option value=\"2026-10-31\">Oct 31 Sa, 2026</option>
<option value=\"2026-11-01\">Nov 01 Su, 2026</option>
<option value=\"2026-11-02\">Nov 02 Mo, 2026</option>
<option value=\"2026-11-03\">Nov 03 Tu, 2026</option>
<option value=\"2026-11-04\">Nov 04 We, 2026</option>
<option value=\"2026-11-05\">Nov 05 Th, 2026</option>
<option value=\"2026-11-06\">Nov 06 Fr, 2026</option>
<option value=\"2026-11-07\">Nov 07 Sa, 2026</option>
<option value=\"2026-11-08\">Nov 08 Su, 2026</option>
<option value=\"2026-11-09\">Nov 09 Mo, 2026</option>
<option value=\"2026-11-10\">Nov 10 Tu, 2026</option>
<option value=\"2026-11-11\">Nov 11 We, 2026</option>
<option value=\"2026-11-12\">Nov 12 Th, 2026</option>
<option value=\"2026-11-13\">Nov 13 Fr, 2026</option>
<option value=\"2026-11-14\">Nov 14 Sa, 2026</option>
<option value=\"2026-11-15\">Nov 15 Su, 2026</option>
<option value=\"2026-11-16\">Nov 16 Mo, 2026</option>
<option value=\"2026-11-17\">Nov 17 Tu, 2026</option>
<option value=\"2026-11-18\">Nov 18 We, 2026</option>
<option value=\"2026-11-19\">Nov 19 Th, 2026</option>
<option value=\"2026-11-20\">Nov 20 Fr, 2026</option>
<option value=\"2026-11-21\">Nov 21 Sa, 2026</option>
<option value=\"2026-11-22\">Nov 22 Su, 2026</option>
<option value=\"2026-11-23\">Nov 23 Mo, 2026</option>
<option value=\"2026-11-24\">Nov 24 Tu, 2026</option>
<option value=\"2026-11-25\">Nov 25 We, 2026</option>
<option value=\"2026-11-26\">Nov 26 Th, 2026</option>
<option value=\"2026-11-27\">Nov 27 Fr, 2026</option>
<option value=\"2026-11-28\">Nov 28 Sa, 2026</option>
<option value=\"2026-11-29\">Nov 29 Su, 2026</option>
<option value=\"2026-11-30\">Nov 30 Mo, 2026</option>
<option value=\"2026-12-01\">Dec 01 Tu, 2026</option>
<option value=\"2026-12-02\">Dec 02 We, 2026</option>
<option value=\"2026-12-03\">Dec 03 Th, 2026</option>
<option value=\"2026-12-04\">Dec 04 Fr, 2026</option>
<option value=\"2026-12-05\">Dec 05 Sa, 2026</option>
<option value=\"2026-12-06\">Dec 06 Su, 2026</option>
<option value=\"2026-12-07\">Dec 07 Mo, 2026</option>
<option value=\"2026-12-08\">Dec 08 Tu, 2026</option>
<option value=\"2026-12-09\">Dec 09 We, 2026</option>
<option value=\"2026-12-10\">Dec 10 Th, 2026</option>
<option value=\"2026-12-11\">Dec 11 Fr, 2026</option>
<option value=\"2026-12-12\">Dec 12 Sa, 2026</option>
<option value=\"2026-12-13\">Dec 13 Su, 2026</option>
<option value=\"2026-12-14\">Dec 14 Mo, 2026</option>
<option value=\"2026-12-15\">Dec 15 Tu, 2026</option>
<option value=\"2026-12-16\">Dec 16 We, 2026</option>
<option value=\"2026-12-17\">Dec 17 Th, 2026</option>
<option value=\"2026-12-18\">Dec 18 Fr, 2026</option>
<option value=\"2026-12-19\">Dec 19 Sa, 2026</option>
<option value=\"2026-12-20\">Dec 20 Su, 2026</option>
<option value=\"2026-12-21\">Dec 21 Mo, 2026</option>
<option value=\"2026-12-22\">Dec 22 Tu, 2026</option>
<option value=\"2026-12-23\">Dec 23 We, 2026</option>
<option value=\"2026-12-24\">Dec 24 Th, 2026</option>
<option value=\"2026-12-25\">Dec 25 Fr, 2026</option>
<option value=\"2026-12-26\">Dec 26 Sa, 2026</option>
<option value=\"2026-12-27\">Dec 27 Su, 2026</option>
<option value=\"2026-12-28\">Dec 28 Mo, 2026</option>
<option value=\"2026-12-29\">Dec 29 Tu, 2026</option>
<option value=\"2026-12-30\">Dec 30 We, 2026</option>
<option value=\"2026-12-31\">Dec 31 Th, 2026</option>
<option value=\"2027-01-01\">Jan 01 Fr, 2027</option>

";

?></contenuto>
</file>
<database>
<tabella>
<nometabella>anni</nometabella>
<colonnetabella>
<nomecolonna>idanni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_periodi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>2025</cmp><cmp>g</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>appartamenti</nometabella>
<colonnetabella>
<nomecolonna>idappartamenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numpiano</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>maxoccupanti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numcasa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>app_vicini</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priorita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priorita2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>letto</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>commento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>15</cmp><cmp>1</cmp><cmp>4</cmp><cmp></cmp><cmp>16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>201</cmp><cmp>2</cmp><cmp>4</cmp><cmp></cmp><cmp>202</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>202</cmp><cmp>2</cmp><cmp>4</cmp><cmp></cmp><cmp>201,203</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>203</cmp><cmp>2</cmp><cmp>4</cmp><cmp></cmp><cmp>202,204</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>204</cmp><cmp>2</cmp><cmp>4</cmp><cmp></cmp><cmp>203</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>16</cmp><cmp>1</cmp><cmp>4</cmp><cmp></cmp><cmp>15,17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>17</cmp><cmp>1</cmp><cmp>4</cmp><cmp></cmp><cmp>16,18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>18</cmp><cmp>1</cmp><cmp>4</cmp><cmp></cmp><cmp>17,19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>19</cmp><cmp>1</cmp><cmp>3</cmp><cmp></cmp><cmp>18,20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>20</cmp><cmp>1</cmp><cmp>4</cmp><cmp></cmp><cmp>19,21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>21</cmp><cmp>1</cmp><cmp>4</cmp><cmp></cmp><cmp>20,209</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>205</cmp><cmp>2</cmp><cmp>4</cmp><cmp></cmp><cmp>206</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>206</cmp><cmp>2</cmp><cmp>4</cmp><cmp></cmp><cmp>205,207</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>207</cmp><cmp>2</cmp><cmp>3</cmp><cmp></cmp><cmp>206,208</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>208</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>207,209</cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>209</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>21,208</cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>210</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>212</cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>212</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>210,213</cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>213</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>212,214</cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>214</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>213,215</cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>215</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>214</cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>clienti</nometabella>
<colonnetabella>
<nomecolonna>idclienti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cognome</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>soprannome</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>sesso</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>titolo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>lingua</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datanascita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cittanascita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>regionenascita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nazionenascita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>documento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>scadenzadoc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipodoc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cittadoc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>regionedoc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nazionedoc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nazionalita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>regione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>citta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>via</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numcivico</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cap</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>telefono</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>telefono2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>telefono3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>fax</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>email</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>email2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>email3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cod_fiscale</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>partita_iva</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>commento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>max_num_ordine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idclienti_compagni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>doc_inviati</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>Johnson</cmp><cmp>John</cmp><cmp></cmp><cmp>m</cmp><cmp></cmp><cmp>ita</cmp><cmp></cmp><cmp>Palermo</cmp><cmp>Palermo</cmp><cmp>Italy</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>Italy</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp>,</cmp><cmp></cmp><cmp>2025-11-16 20:12:57</cmp><cmp>127.0.0.1</cmp><cmp>1</cmp></riga>
<riga><cmp>2</cmp><cmp>Doe</cmp><cmp>Jane</cmp><cmp></cmp><cmp>f</cmp><cmp></cmp><cmp>en</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>United Arab Emirates</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp>,</cmp><cmp></cmp><cmp>2025-11-23 20:49:15</cmp><cmp>127.0.0.1</cmp><cmp>1</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>relclienti</nometabella>
<colonnetabella>
<nomecolonna>idclienti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numero</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo1</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo4</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo5</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo6</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo7</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo8</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>personalizza</nometabella>
<colonnetabella>
<nomecolonna>idpersonalizza</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idutente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valpersonalizza</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valpersonalizza_num</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>col_tab_tutte_prenota</cmp><cmp>1</cmp><cmp>nu#@&cg#@&in#@&fi#@&tc#@&ca#@&pa#@&ap#@&pe#@&co</cmp><cmp></cmp></riga>
<riga><cmp>rig_tab_tutte_prenota</cmp><cmp>1</cmp><cmp>to#@&ta#@&ca#@&pc</cmp><cmp></cmp></riga>
<riga><cmp>modo_invio_email</cmp><cmp>1</cmp><cmp>locale</cmp><cmp></cmp></riga>
<riga><cmp>maschera_email</cmp><cmp>1</cmp><cmp>NO</cmp><cmp></cmp></riga>
<riga><cmp>dati_struttura</cmp><cmp>1</cmp><cmp>Villa Annunziata#@&Casa per Ferie#@&annunziata.villa@gmail.com#@&#@&http://www.domenicaneannunziata.it#@&#@&Italy#@&Roma#@&Via di Villa Maggiorani, 9#@&00168#@&+39 06 305 34 05 / +39 06 305 22 59#@&#@&IT058091B7WGD28RJI#@&IT02139731000#@&#@&#@&Roma</cmp><cmp></cmp></riga>
<riga><cmp>valuta</cmp><cmp>1</cmp><cmp>Euros</cmp><cmp></cmp></riga>
<riga><cmp>arrotond_predef</cmp><cmp>1</cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>arrotond_tasse</cmp><cmp>1</cmp><cmp>0.01</cmp><cmp></cmp></riga>
<riga><cmp>stile_soldi</cmp><cmp>1</cmp><cmp>usa</cmp><cmp></cmp></riga>
<riga><cmp>costi_agg_in_tab_prenota</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>aggiunta_tronca_nomi_tab1</cmp><cmp>1</cmp><cmp></cmp><cmp>-2</cmp></riga>
<riga><cmp>linee_ripeti_date_tab_mesi</cmp><cmp>1</cmp><cmp></cmp><cmp>25</cmp></riga>
<riga><cmp>mostra_giorni_tab_mesi</cmp><cmp>1</cmp><cmp>SI</cmp><cmp></cmp></riga>
<riga><cmp>colori_tab_mesi</cmp><cmp>1</cmp><cmp>#70C6D4,#FFD800,#FF9900,#FF3115</cmp><cmp></cmp></riga>
<riga><cmp>num_linee_tab2_prenota</cmp><cmp>1</cmp><cmp></cmp><cmp>30</cmp></riga>
<riga><cmp>nomi_contratti</cmp><cmp>1</cmp><cmp>1#?&Example#@&2#?&Invoice#@&3#?&Invoice - rtf#@&4#?&Last payment receipt#@&5#?&Receipt - rtf#@&6#?&Availability email#@&7#?&Confirm reservation email#@&8#?&Welcome email#@&9#?&Cleaning Report#@&10#?&Export clients data#@&11#?&Export reservations</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_tutte_prenota</cmp><cmp>1</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>selezione_tab_tutte_prenota</cmp><cmp>1</cmp><cmp>tutte</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_tutti_clienti</cmp><cmp>1</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>num_righe_tab_casse</cmp><cmp>1</cmp><cmp></cmp><cmp>50</cmp></riga>
<riga><cmp>tot_giornalero_tab_casse</cmp><cmp>1</cmp><cmp>gior,mens,tab</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_messaggi</cmp><cmp>1</cmp><cmp></cmp><cmp>80</cmp></riga>
<riga><cmp>num_righe_tab_doc_salvati</cmp><cmp>1</cmp><cmp></cmp><cmp>100</cmp></riga>
<riga><cmp>num_righe_tab_storia_soldi</cmp><cmp>1</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>stile_data</cmp><cmp>1</cmp><cmp>europa</cmp><cmp></cmp></riga>
<riga><cmp>stile_nomi</cmp><cmp>1</cmp><cmp>ma1or</cmp><cmp></cmp></riga>
<riga><cmp>num_categorie_persone</cmp><cmp>1</cmp><cmp></cmp><cmp>1</cmp></riga>
<riga><cmp>minuti_durata_sessione</cmp><cmp>1</cmp><cmp></cmp><cmp>90</cmp></riga>
<riga><cmp>usa_cookies</cmp><cmp>1</cmp><cmp></cmp><cmp>0</cmp></riga>
<riga><cmp>minuti_durata_insprenota</cmp><cmp>1</cmp><cmp></cmp><cmp>10</cmp></riga>
<riga><cmp>ore_anticipa_periodo_corrente</cmp><cmp>1</cmp><cmp></cmp><cmp>0</cmp></riga>
<riga><cmp>tutti_fissi</cmp><cmp>1</cmp><cmp>10</cmp><cmp></cmp></riga>
<riga><cmp>auto_crea_anno</cmp><cmp>1</cmp><cmp>SI</cmp><cmp></cmp></riga>
<riga><cmp>metodi_pagamento</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>origini_prenota</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>attiva_checkin</cmp><cmp>1</cmp><cmp>NO</cmp><cmp></cmp></riga>
<riga><cmp>mostra_quadro_disp</cmp><cmp>1</cmp><cmp>reg2</cmp><cmp></cmp></riga>
<riga><cmp>subordinazione</cmp><cmp>1</cmp><cmp>NO</cmp><cmp></cmp></riga>
<riga><cmp>percorso_cartella_modello</cmp><cmp>1</cmp><cmp>./dati</cmp><cmp></cmp></riga>
<riga><cmp>gest_cvc</cmp><cmp>1</cmp><cmp>NO</cmp><cmp></cmp></riga>
<riga><cmp>ordine_inventario</cmp><cmp>1</cmp><cmp>alf</cmp><cmp></cmp></riga>
<riga><cmp>tasti_pos</cmp><cmp>1</cmp><cmp>x2;x10;s;+1;+2;+3;+4;+5;+6;+7;+8;+9;s;-1</cmp><cmp></cmp></riga>
<riga><cmp>ultime_sel_ins_prezzi</cmp><cmp>1</cmp><cmp>1,2025,2025-01-01,2025-12-31</cmp><cmp></cmp></riga>
<riga><cmp>col_tab_tutte_prenota</cmp><cmp>2</cmp><cmp>nu#@&cg#@&in#@&fi#@&tc#@&ca#@&pa#@&ap#@&pe#@&co</cmp><cmp></cmp></riga>
<riga><cmp>rig_tab_tutte_prenota</cmp><cmp>2</cmp><cmp>to#@&ta#@&ca#@&pc</cmp><cmp></cmp></riga>
<riga><cmp>dati_struttura</cmp><cmp>2</cmp><cmp>#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&</cmp><cmp></cmp></riga>
<riga><cmp>valuta</cmp><cmp>2</cmp><cmp>Euro</cmp><cmp></cmp></riga>
<riga><cmp>arrotond_predef</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>arrotond_tasse</cmp><cmp>2</cmp><cmp>0.01</cmp><cmp></cmp></riga>
<riga><cmp>stile_soldi</cmp><cmp>2</cmp><cmp>europa</cmp><cmp></cmp></riga>
<riga><cmp>costi_agg_in_tab_prenota</cmp><cmp>2</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>aggiunta_tronca_nomi_tab1</cmp><cmp>2</cmp><cmp></cmp><cmp>-2</cmp></riga>
<riga><cmp>linee_ripeti_date_tab_mesi</cmp><cmp>2</cmp><cmp></cmp><cmp>25</cmp></riga>
<riga><cmp>mostra_giorni_tab_mesi</cmp><cmp>2</cmp><cmp>SI</cmp><cmp></cmp></riga>
<riga><cmp>colori_tab_mesi</cmp><cmp>2</cmp><cmp>#70C6D4,#FFD800,#FF9900,#FF3115</cmp><cmp></cmp></riga>
<riga><cmp>num_linee_tab2_prenota</cmp><cmp>2</cmp><cmp></cmp><cmp>30</cmp></riga>
<riga><cmp>nomi_contratti</cmp><cmp>2</cmp><cmp>1#?&Example#@&2#?&Invoice#@&3#?&Invoice - rtf#@&4#?&Last payment receipt#@&5#?&Receipt - rtf#@&6#?&Availability email#@&7#?&Confirm reservation email#@&8#?&Welcome email#@&9#?&Cleaning Report#@&10#?&Export clients data#@&11#?&Export reservations</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_tutte_prenota</cmp><cmp>2</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>selezione_tab_tutte_prenota</cmp><cmp>2</cmp><cmp>tutte</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_tutti_clienti</cmp><cmp>2</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>num_righe_tab_messaggi</cmp><cmp>2</cmp><cmp></cmp><cmp>80</cmp></riga>
<riga><cmp>num_righe_tab_casse</cmp><cmp>2</cmp><cmp></cmp><cmp>50</cmp></riga>
<riga><cmp>tot_giornalero_tab_casse</cmp><cmp>2</cmp><cmp>gior,mens,tab</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_doc_salvati</cmp><cmp>2</cmp><cmp></cmp><cmp>100</cmp></riga>
<riga><cmp>num_righe_tab_storia_soldi</cmp><cmp>2</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>stile_data</cmp><cmp>2</cmp><cmp>europa</cmp><cmp></cmp></riga>
<riga><cmp>stile_nomi</cmp><cmp>2</cmp><cmp>ma1or</cmp><cmp></cmp></riga>
<riga><cmp>num_categorie_persone</cmp><cmp>2</cmp><cmp></cmp><cmp>1</cmp></riga>
<riga><cmp>ore_anticipa_periodo_corrente</cmp><cmp>2</cmp><cmp></cmp><cmp>0</cmp></riga>
<riga><cmp>metodi_pagamento</cmp><cmp>2</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>origini_prenota</cmp><cmp>2</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>attiva_checkin</cmp><cmp>2</cmp><cmp>NO</cmp><cmp></cmp></riga>
<riga><cmp>mostra_quadro_disp</cmp><cmp>2</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>ordine_inventario</cmp><cmp>2</cmp><cmp>alf</cmp><cmp></cmp></riga>
<riga><cmp>tasti_pos</cmp><cmp>2</cmp><cmp>x2;x10;s;+1;+2;+3;+4;+5;+6;+7;+8;+9;s;-1</cmp><cmp></cmp></riga>
<riga><cmp>col_tab_tutte_prenota</cmp><cmp>3</cmp><cmp>nu#@&cg#@&in#@&fi#@&tc#@&ca#@&pa#@&ap#@&pe#@&co</cmp><cmp></cmp></riga>
<riga><cmp>rig_tab_tutte_prenota</cmp><cmp>3</cmp><cmp>to#@&ta#@&ca#@&pc</cmp><cmp></cmp></riga>
<riga><cmp>dati_struttura</cmp><cmp>3</cmp><cmp>#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&</cmp><cmp></cmp></riga>
<riga><cmp>valuta</cmp><cmp>3</cmp><cmp>Euro</cmp><cmp></cmp></riga>
<riga><cmp>arrotond_predef</cmp><cmp>3</cmp><cmp>1</cmp><cmp></cmp></riga>
<riga><cmp>arrotond_tasse</cmp><cmp>3</cmp><cmp>0.01</cmp><cmp></cmp></riga>
<riga><cmp>stile_soldi</cmp><cmp>3</cmp><cmp>europa</cmp><cmp></cmp></riga>
<riga><cmp>costi_agg_in_tab_prenota</cmp><cmp>3</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>aggiunta_tronca_nomi_tab1</cmp><cmp>3</cmp><cmp></cmp><cmp>-2</cmp></riga>
<riga><cmp>linee_ripeti_date_tab_mesi</cmp><cmp>3</cmp><cmp></cmp><cmp>25</cmp></riga>
<riga><cmp>mostra_giorni_tab_mesi</cmp><cmp>3</cmp><cmp>SI</cmp><cmp></cmp></riga>
<riga><cmp>colori_tab_mesi</cmp><cmp>3</cmp><cmp>#70C6D4,#FFD800,#FF9900,#FF3115</cmp><cmp></cmp></riga>
<riga><cmp>num_linee_tab2_prenota</cmp><cmp>3</cmp><cmp></cmp><cmp>30</cmp></riga>
<riga><cmp>nomi_contratti</cmp><cmp>3</cmp><cmp>1#?&Example#@&2#?&Invoice#@&3#?&Invoice - rtf#@&4#?&Last payment receipt#@&5#?&Receipt - rtf#@&6#?&Availability email#@&7#?&Confirm reservation email#@&8#?&Welcome email#@&9#?&Cleaning Report#@&10#?&Export clients data#@&11#?&Export reservations</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_tutte_prenota</cmp><cmp>3</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>selezione_tab_tutte_prenota</cmp><cmp>3</cmp><cmp>tutte</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_tutti_clienti</cmp><cmp>3</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>num_righe_tab_messaggi</cmp><cmp>3</cmp><cmp></cmp><cmp>80</cmp></riga>
<riga><cmp>num_righe_tab_casse</cmp><cmp>3</cmp><cmp></cmp><cmp>50</cmp></riga>
<riga><cmp>tot_giornalero_tab_casse</cmp><cmp>3</cmp><cmp>gior,mens,tab</cmp><cmp></cmp></riga>
<riga><cmp>num_righe_tab_doc_salvati</cmp><cmp>3</cmp><cmp></cmp><cmp>100</cmp></riga>
<riga><cmp>num_righe_tab_storia_soldi</cmp><cmp>3</cmp><cmp></cmp><cmp>200</cmp></riga>
<riga><cmp>stile_data</cmp><cmp>3</cmp><cmp>europa</cmp><cmp></cmp></riga>
<riga><cmp>stile_nomi</cmp><cmp>3</cmp><cmp>ma1or</cmp><cmp></cmp></riga>
<riga><cmp>num_categorie_persone</cmp><cmp>3</cmp><cmp></cmp><cmp>1</cmp></riga>
<riga><cmp>ore_anticipa_periodo_corrente</cmp><cmp>3</cmp><cmp></cmp><cmp>0</cmp></riga>
<riga><cmp>metodi_pagamento</cmp><cmp>3</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>origini_prenota</cmp><cmp>3</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>attiva_checkin</cmp><cmp>3</cmp><cmp>NO</cmp><cmp></cmp></riga>
<riga><cmp>mostra_quadro_disp</cmp><cmp>3</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>ordine_inventario</cmp><cmp>3</cmp><cmp>alf</cmp><cmp></cmp></riga>
<riga><cmp>tasti_pos</cmp><cmp>3</cmp><cmp>x2;x10;s;+1;+2;+3;+4;+5;+6;+7;+8;+9;s;-1</cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>versioni</nometabella>
<colonnetabella>
<nomecolonna>idversioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>num_versione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>3.07</cmp></riga>
<riga><cmp>2</cmp><cmp>128</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>utenti</nometabella>
<colonnetabella>
<nomecolonna>idutenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_utente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>password</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>salt</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_pass</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>admin</cmp><cmp>632e213a6788ad7b860eef01c464b0f3</cmp><cmp>#cxn39zDLEwtC2AGs@Bv</cmp><cmp>5</cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>Tina</cmp><cmp></cmp><cmp></cmp><cmp>n</cmp><cmp>2025-11-16 20:27:25</cmp><cmp>127.0.0.1</cmp></riga>
<riga><cmp>3</cmp><cmp>Rosario</cmp><cmp></cmp><cmp></cmp><cmp>n</cmp><cmp>2025-11-16 20:27:39</cmp><cmp>127.0.0.1</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>gruppi</nometabella>
<colonnetabella>
<nomecolonna>idgruppi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_gruppo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>privilegi</nometabella>
<colonnetabella>
<nomecolonna>idutente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>anno</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>regole1_consentite</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffe_consentite</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>costi_agg_consentiti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>contratti_consentiti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>casse_consentite</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cassa_pagamenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_ins_prenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_mod_prenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_mod_pers</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_ins_clienti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>prefisso_clienti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_ins_costi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_vedi_tab</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_ins_tariffe</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_ins_regole</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_messaggi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>priv_inventario</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>n,</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>ssssssssssp</cmp><cmp>sssss</cmp><cmp>n,</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>ss</cmp><cmp>sssssssss</cmp></riga>
<riga><cmp>3</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>n,</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>nnnnnnnnnnn</cmp><cmp>nnnss</cmp><cmp>n,</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>nn</cmp><cmp>nnnnnnnnn</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>sessioni</nometabella>
<colonnetabella>
<nomecolonna>idsessioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idcliente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idutente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>indirizzo_ip</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_conn</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>user_agent</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>ultimo_accesso</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>20251123153215njbiVJC7EtuomTAk123</cmp><cmp></cmp><cmp>1</cmp><cmp>127.0.0.1</cmp><cmp>HTTP</cmp><cmp>Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0</cmp><cmp>2025-11-23 21:21:10</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>transazioni</nometabella>
<colonnetabella>
<nomecolonna>idtransazioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idcliente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idsessione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_transazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>anno</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>spostamenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione1</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione4</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione5</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione6</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione7</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione8</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione9</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione10</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione11</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione12</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione13</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione14</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione15</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione16</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione17</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione18</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione19</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione20</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione21</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione22</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>ultimo_accesso</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>20251123183730qJiewSCy124</cmp><cmp></cmp><cmp>20251123153215njbiVJC7EtuomTAk123</cmp><cmp>tab_p</cmp><cmp>2025</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>iddatainizio,idprenota</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp>tutte</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>2025-11-23 20:53:53</cmp></riga>
<riga><cmp>20251123204833S9ovnpvB127</cmp><cmp></cmp><cmp>20251123153215njbiVJC7EtuomTAk123</cmp><cmp>ins_p</cmp><cmp>2025</cmp><cmp></cmp><cmp>1</cmp><cmp>332</cmp><cmp>346</cmp><cmp>201</cmp><cmp>tariffa1</cmp><cmp>3</cmp><cmp>v</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>,</cmp><cmp>2</cmp><cmp>0</cmp><cmp>a:1:{i:1;a:1:{i:1;a:1:{i:1;i:2;}}}</cmp><cmp>a:1:{i:1;a:1:{i:1;a:1:{i:1;s:1:"1";}}}</cmp><cmp>a:0:{}</cmp><cmp>a:0:{}</cmp><cmp>,,</cmp><cmp></cmp><cmp>a:2:{i:1;a:0:{}s:13:"numpersone_nr";a:1:{i:1;a:0:{}}}</cmp><cmp>a:3:{s:19:"num_app_reali_costo";a:0:{}s:19:"diff_aggiungi_letti";a:0:{}s:14:"posti_mancanti";a:1:{i:1;i:0;}}</cmp><cmp>2025-11-23 20:49:15</cmp></riga>
<riga><cmp>20251123204921G5rUIs37128</cmp><cmp></cmp><cmp></cmp><cmp>rate_limit</cmp><cmp></cmp><cmp></cmp><cmp>login_attempts_f528764d624db129b32c21fbca0cb8d6</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>2025-11-23 20:49:21</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>transazioniweb</nometabella>
<colonnetabella>
<nomecolonna>idtransazioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idcliente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idsessione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_transazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>anno</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>spostamenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione1</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione4</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione5</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione6</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione7</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione8</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione9</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione10</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione11</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione12</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione13</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione14</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione15</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione16</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione17</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione18</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione19</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione20</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione21</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_transazione22</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>ultimo_accesso</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>2</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>100</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>descrizioni</nometabella>
<colonnetabella>
<nomecolonna>nome</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>lingua</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numero</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>nazioni</nometabella>
<colonnetabella>
<nomecolonna>idnazioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_nazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_nazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice2_nazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice3_nazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>Austria</cmp><cmp>AT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>Belgium</cmp><cmp>BE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>Czech Republic</cmp><cmp>CZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>4</cmp><cmp>Cyprus</cmp><cmp>CY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>5</cmp><cmp>Denmark</cmp><cmp>DK</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>6</cmp><cmp>Estonia</cmp><cmp>EE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>7</cmp><cmp>Finland</cmp><cmp>FI</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>8</cmp><cmp>France</cmp><cmp>FR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>9</cmp><cmp>Germany</cmp><cmp>DE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>10</cmp><cmp>Greece</cmp><cmp>GR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>11</cmp><cmp>Ireland</cmp><cmp>IE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>12</cmp><cmp>Latvia</cmp><cmp>LV</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>13</cmp><cmp>Lithuania</cmp><cmp>LT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>14</cmp><cmp>Luxembourg</cmp><cmp>LU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>15</cmp><cmp>Malta</cmp><cmp>MT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>16</cmp><cmp>Netherlands</cmp><cmp>NL</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>17</cmp><cmp>Poland</cmp><cmp>PL</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>18</cmp><cmp>Portugal</cmp><cmp>PT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>19</cmp><cmp>United Kingdom</cmp><cmp>GB</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>20</cmp><cmp>Slovakia</cmp><cmp>SK</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>21</cmp><cmp>Slovenia</cmp><cmp>SI</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>22</cmp><cmp>Spain</cmp><cmp>ES</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>23</cmp><cmp>Sweden</cmp><cmp>SE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>24</cmp><cmp>Hungary</cmp><cmp>HU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>25</cmp><cmp>Albania</cmp><cmp>AL</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>26</cmp><cmp>Andorra</cmp><cmp>AD</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>27</cmp><cmp>Belarus</cmp><cmp>BY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>28</cmp><cmp>Bosnia And Herzegovina</cmp><cmp>BA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>29</cmp><cmp>Bulgaria</cmp><cmp>BG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>30</cmp><cmp>Croatia</cmp><cmp>HR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>31</cmp><cmp>Iceland</cmp><cmp>IS</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>32</cmp><cmp>Liechtenstein</cmp><cmp>LI</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>33</cmp><cmp>Macedonia</cmp><cmp>MK</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>34</cmp><cmp>Moldova</cmp><cmp>MD</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>35</cmp><cmp>Monaco</cmp><cmp>MC</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>36</cmp><cmp>Montenegro</cmp><cmp>ME</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>37</cmp><cmp>Norway</cmp><cmp>NO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>38</cmp><cmp>Romania</cmp><cmp>RO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>39</cmp><cmp>Russia</cmp><cmp>RU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>40</cmp><cmp>San Marino</cmp><cmp>SM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>41</cmp><cmp>Vatican City</cmp><cmp>VA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>42</cmp><cmp>Serbia</cmp><cmp>YU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>43</cmp><cmp>Switzerland</cmp><cmp>CH</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>44</cmp><cmp>Turkey</cmp><cmp>TR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>45</cmp><cmp>Ukraine</cmp><cmp>UA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>46</cmp><cmp>Afghanistan</cmp><cmp>AF</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>47</cmp><cmp>Saudi Arabia</cmp><cmp>SA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>48</cmp><cmp>Armenia</cmp><cmp>AM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>49</cmp><cmp>Azerbaijan</cmp><cmp>AZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>50</cmp><cmp>Bahrein</cmp><cmp>BH</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>51</cmp><cmp>Bangladesh</cmp><cmp>BD</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>52</cmp><cmp>Bhutan</cmp><cmp>BT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>53</cmp><cmp>Brunei</cmp><cmp>BN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>54</cmp><cmp>Cambodia</cmp><cmp>KH</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>55</cmp><cmp>China</cmp><cmp>CN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>56</cmp><cmp>South Korea</cmp><cmp>KR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>57</cmp><cmp>North Korea</cmp><cmp>KP</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>58</cmp><cmp>United Arab Emirates</cmp><cmp>AE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>59</cmp><cmp>Philippines</cmp><cmp>PH</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>60</cmp><cmp>Georgia</cmp><cmp>GE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>61</cmp><cmp>Japan</cmp><cmp>JP</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>62</cmp><cmp>Jordan</cmp><cmp>JO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>63</cmp><cmp>India</cmp><cmp>IN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>64</cmp><cmp>Indonesia</cmp><cmp>ID</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>65</cmp><cmp>Iran</cmp><cmp>IR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>66</cmp><cmp>Iraq</cmp><cmp>IQ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>67</cmp><cmp>Israel</cmp><cmp>IL</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>68</cmp><cmp>Kazakhstan</cmp><cmp>KZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>69</cmp><cmp>Kyrgyzstan</cmp><cmp>KG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>70</cmp><cmp>Kuwait</cmp><cmp>KW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>71</cmp><cmp>Laos</cmp><cmp>LA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>72</cmp><cmp>Lebanon</cmp><cmp>LB</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>73</cmp><cmp>Malaysia</cmp><cmp>MY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>74</cmp><cmp>Maldives</cmp><cmp>MV</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>75</cmp><cmp>Mongolia</cmp><cmp>MN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>76</cmp><cmp>Myanmar</cmp><cmp>MM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>77</cmp><cmp>Nepal</cmp><cmp>NP</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>78</cmp><cmp>Oman</cmp><cmp>OM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>79</cmp><cmp>Pakistan</cmp><cmp>PK</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>80</cmp><cmp>Qatar</cmp><cmp>QA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>81</cmp><cmp>Singapore</cmp><cmp>SG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>82</cmp><cmp>Syria</cmp><cmp>SY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>83</cmp><cmp>Sri Lanka</cmp><cmp>LK</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>84</cmp><cmp>Tajikistan</cmp><cmp>TJ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>85</cmp><cmp>Taiwan</cmp><cmp>TW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>86</cmp><cmp>Palestine</cmp><cmp>PS</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>87</cmp><cmp>Thailand</cmp><cmp>TH</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>88</cmp><cmp>Timor-Leste</cmp><cmp>TL</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>89</cmp><cmp>Turkmenistan</cmp><cmp>TM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>90</cmp><cmp>Uzbekistan</cmp><cmp>UZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>91</cmp><cmp>Vietnam</cmp><cmp>VN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>92</cmp><cmp>Yemen</cmp><cmp>YE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>93</cmp><cmp>Algeria</cmp><cmp>DZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>94</cmp><cmp>Angola</cmp><cmp>AO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>95</cmp><cmp>Benin</cmp><cmp>BJ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>96</cmp><cmp>Botswana</cmp><cmp>BW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>97</cmp><cmp>Burkina Faso</cmp><cmp>BF</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>98</cmp><cmp>Burundi</cmp><cmp>BI</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>99</cmp><cmp>Cameroon</cmp><cmp>CM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>100</cmp><cmp>Cape Verde</cmp><cmp>CV</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>101</cmp><cmp>Central African Republic</cmp><cmp>CF</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>102</cmp><cmp>Chad</cmp><cmp>TD</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>103</cmp><cmp>Comoros</cmp><cmp>KM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>104</cmp><cmp>Republic Of Congo</cmp><cmp>CG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>105</cmp><cmp>Dem. Rep. Of Congo</cmp><cmp>CD</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>106</cmp><cmp>Côte D'Ivoire</cmp><cmp>CI</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>107</cmp><cmp>Egypt</cmp><cmp>EG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>108</cmp><cmp>Eritrea</cmp><cmp>ER</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>109</cmp><cmp>Ethiopia</cmp><cmp>ET</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>110</cmp><cmp>Gabon</cmp><cmp>GA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>111</cmp><cmp>Gambia</cmp><cmp>GM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>112</cmp><cmp>Ghana</cmp><cmp>GH</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>113</cmp><cmp>Djibouti</cmp><cmp>DJ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>114</cmp><cmp>Guinea</cmp><cmp>GN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>115</cmp><cmp>Guinea-Bissau</cmp><cmp>GW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>116</cmp><cmp>Equatorial Guinea</cmp><cmp>GQ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>117</cmp><cmp>Kenya</cmp><cmp>KE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>118</cmp><cmp>Lesotho</cmp><cmp>LS</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>119</cmp><cmp>Liberia</cmp><cmp>LR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>120</cmp><cmp>Libya</cmp><cmp>LY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>121</cmp><cmp>Madagascar</cmp><cmp>MG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>122</cmp><cmp>Malawi</cmp><cmp>MW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>123</cmp><cmp>Mali</cmp><cmp>ML</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>124</cmp><cmp>Morocco</cmp><cmp>MA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>125</cmp><cmp>Mauritania</cmp><cmp>MR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>126</cmp><cmp>Mauritius</cmp><cmp>MU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>127</cmp><cmp>Mozambique</cmp><cmp>MZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>128</cmp><cmp>Namibia</cmp><cmp>NA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>129</cmp><cmp>Niger</cmp><cmp>NE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>130</cmp><cmp>Nigeria</cmp><cmp>NG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>131</cmp><cmp>Rwanda</cmp><cmp>RW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>132</cmp><cmp>São Tomé And Príncipe</cmp><cmp>ST</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>133</cmp><cmp>Senegal</cmp><cmp>SN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>134</cmp><cmp>Seychelles</cmp><cmp>SC</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>135</cmp><cmp>Sierra Leone</cmp><cmp>SL</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>136</cmp><cmp>Somalia</cmp><cmp>SO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>137</cmp><cmp>South Africa</cmp><cmp>ZA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>138</cmp><cmp>Sudan</cmp><cmp>SD</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>139</cmp><cmp>Swaziland</cmp><cmp>SZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>140</cmp><cmp>Tanzania</cmp><cmp>TZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>141</cmp><cmp>Togo</cmp><cmp>TG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>142</cmp><cmp>Tunisia</cmp><cmp>TN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>143</cmp><cmp>Uganda</cmp><cmp>UG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>144</cmp><cmp>Zambia</cmp><cmp>ZM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>145</cmp><cmp>Zimbabwe</cmp><cmp>ZW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>146</cmp><cmp>Antigua And Barbuda</cmp><cmp>AG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>147</cmp><cmp>Argentina</cmp><cmp>AR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>148</cmp><cmp>Bahamas</cmp><cmp>BS</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>149</cmp><cmp>Barbados</cmp><cmp>BB</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>150</cmp><cmp>Belize</cmp><cmp>BZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>151</cmp><cmp>Bolivia</cmp><cmp>BO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>152</cmp><cmp>Brazil</cmp><cmp>BR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>153</cmp><cmp>Canada</cmp><cmp>CA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>154</cmp><cmp>Chile</cmp><cmp>CL</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>155</cmp><cmp>Colombia</cmp><cmp>CO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>156</cmp><cmp>Costa Rica</cmp><cmp>CR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>157</cmp><cmp>Cuba</cmp><cmp>CU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>158</cmp><cmp>Dominica</cmp><cmp>DM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>159</cmp><cmp>Dominican Republic</cmp><cmp>DO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>160</cmp><cmp>Ecuador</cmp><cmp>EC</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>161</cmp><cmp>El Salvador</cmp><cmp>SV</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>162</cmp><cmp>Jamaica</cmp><cmp>JM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>163</cmp><cmp>Grenada</cmp><cmp>GD</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>164</cmp><cmp>Guatemala</cmp><cmp>GT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>165</cmp><cmp>Guyana</cmp><cmp>GY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>166</cmp><cmp>Haiti</cmp><cmp>HT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>167</cmp><cmp>Honduras</cmp><cmp>HN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>168</cmp><cmp>Mexico</cmp><cmp>MX</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>169</cmp><cmp>Nicaragua</cmp><cmp>NI</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>170</cmp><cmp>Panama</cmp><cmp>PA</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>171</cmp><cmp>Paraguay</cmp><cmp>PY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>172</cmp><cmp>Peru</cmp><cmp>PE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>173</cmp><cmp>Saint Kitts And Nevis</cmp><cmp>KN</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>174</cmp><cmp>Saint Lucia</cmp><cmp>LC</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>175</cmp><cmp>Saint Vincent And Grenadines</cmp><cmp>VC</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>176</cmp><cmp>United States Of America</cmp><cmp>US</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>177</cmp><cmp>Suriname</cmp><cmp>SR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>178</cmp><cmp>Trinidad And Tobago</cmp><cmp>TT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>179</cmp><cmp>Uruguay</cmp><cmp>UY</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>180</cmp><cmp>Venezuela</cmp><cmp>VE</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>181</cmp><cmp>Australia</cmp><cmp>AU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>182</cmp><cmp>Fiji</cmp><cmp>FJ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>183</cmp><cmp>Kiribati</cmp><cmp>KI</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>184</cmp><cmp>Marshall Islands</cmp><cmp>MH</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>185</cmp><cmp>Micronesia</cmp><cmp>FM</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>186</cmp><cmp>Nauru</cmp><cmp>NR</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>187</cmp><cmp>New Zealand</cmp><cmp>NZ</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>188</cmp><cmp>Palau</cmp><cmp>PW</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>189</cmp><cmp>Papua New Guinea</cmp><cmp>PG</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>190</cmp><cmp>Solomon Islands</cmp><cmp>SB</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>191</cmp><cmp>Samoa</cmp><cmp>WS</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>192</cmp><cmp>Tonga</cmp><cmp>TO</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>193</cmp><cmp>Tuvalu</cmp><cmp>TV</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>194</cmp><cmp>Vanuatu</cmp><cmp>VU</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>195</cmp><cmp>Italy</cmp><cmp>IT</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>196</cmp><cmp>Kosovo</cmp><cmp>XK</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>197</cmp><cmp>South Sudan</cmp><cmp>SS</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>regioni</nometabella>
<colonnetabella>
<nomecolonna>idregioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_regione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_regione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice2_regione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice3_regione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>citta</nometabella>
<colonnetabella>
<nomecolonna>idcitta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_citta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_citta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice2_citta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice3_citta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>documentiid</nometabella>
<colonnetabella>
<nomecolonna>iddocumentiid</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_documentoid</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_documentoid</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice2_documentoid</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice3_documentoid</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>parentele</nometabella>
<colonnetabella>
<nomecolonna>idparentele</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_parentela</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_parentela</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice2_parentela</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice3_parentela</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>relutenti</nometabella>
<colonnetabella>
<nomecolonna>idutente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idnazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idregione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idcitta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>iddocumentoid</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idparentela</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idsup</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>predef</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>2</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>3</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>4</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>5</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>6</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>7</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>8</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>9</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>32</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>33</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>34</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>35</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>36</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>37</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>38</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>39</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>40</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>41</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>42</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>43</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>44</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>45</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>46</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>47</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>48</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>49</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>50</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>51</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>52</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>53</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>54</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>55</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>56</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>57</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>58</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>59</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>60</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>61</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>62</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>63</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>64</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>65</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>66</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>67</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>68</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>69</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>70</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>71</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>72</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>73</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>74</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>75</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>76</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>77</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>78</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>79</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>80</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>81</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>82</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>83</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>84</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>85</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>86</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>87</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>88</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>89</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>90</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>91</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>92</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>93</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>94</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>95</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>96</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>97</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>98</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>99</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>100</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>101</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>102</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>103</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>104</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>105</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>106</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>107</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>108</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>109</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>110</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>111</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>112</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>113</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>114</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>115</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>116</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>117</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>118</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>119</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>120</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>121</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>122</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>123</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>124</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>125</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>126</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>127</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>128</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>129</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>130</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>131</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>132</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>133</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>134</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>135</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>136</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>137</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>138</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>139</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>140</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>141</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>142</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>143</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>144</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>145</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>146</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>147</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>148</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>149</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>150</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>151</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>152</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>153</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>154</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>155</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>156</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>157</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>158</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>159</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>160</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>161</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>162</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>163</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>164</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>165</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>166</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>167</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>168</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>169</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>170</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>171</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>172</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>173</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>174</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>175</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>176</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>177</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>178</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>179</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>180</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>181</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>182</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>183</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>184</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>185</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>186</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>187</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>188</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>189</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>190</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>191</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>192</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>193</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>194</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>195</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>196</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>1</cmp><cmp>197</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>46</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>93</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>94</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>146</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>147</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>48</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>181</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>49</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>148</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>50</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>51</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>149</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>2</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>150</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>95</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>52</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>151</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>96</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>152</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>53</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>97</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>98</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>54</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>99</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>153</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>100</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>101</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>102</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>154</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>55</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>155</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>103</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>156</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>157</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>4</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>3</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>106</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>105</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>5</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>113</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>158</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>159</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>160</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>107</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>161</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>116</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>108</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>6</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>109</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>182</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>7</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>8</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>110</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>111</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>60</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>9</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>112</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>163</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>164</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>114</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>115</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>165</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>166</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>167</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>63</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>64</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>65</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>66</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>67</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>195</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>162</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>61</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>62</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>68</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>117</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>183</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>196</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>70</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>69</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>71</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>72</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>118</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>119</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>120</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>32</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>33</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>121</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>122</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>73</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>74</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>123</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>184</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>125</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>126</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>168</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>185</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>34</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>35</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>75</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>36</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>124</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>127</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>76</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>128</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>186</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>77</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>187</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>169</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>129</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>130</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>57</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>37</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>78</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>79</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>188</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>86</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>170</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>189</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>171</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>172</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>59</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>80</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>104</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>38</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>39</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>131</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>173</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>174</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>175</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>191</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>40</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>47</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>133</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>42</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>134</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>135</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>81</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>190</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>136</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>137</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>56</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>197</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>83</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>138</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>177</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>139</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>43</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>82</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>132</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>85</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>84</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>140</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>87</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>88</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>141</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>192</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>178</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>142</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>44</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>89</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>193</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>143</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>45</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>58</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>176</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>179</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>90</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>194</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>41</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>180</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>91</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>92</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>144</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>145</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>46</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>93</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>94</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>146</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>147</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>48</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>181</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>49</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>148</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>50</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>51</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>149</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>2</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>150</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>95</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>52</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>151</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>96</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>152</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>53</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>97</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>98</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>54</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>99</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>153</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>100</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>101</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>102</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>154</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>55</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>155</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>103</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>156</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>157</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>4</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>3</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>106</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>105</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>5</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>113</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>158</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>159</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>160</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>107</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>161</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>116</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>108</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>6</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>109</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>182</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>7</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>8</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>110</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>111</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>60</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>9</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>112</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>163</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>164</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>114</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>115</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>165</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>166</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>167</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>63</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>64</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>65</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>66</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>67</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>195</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>162</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>61</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>62</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>68</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>117</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>183</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>196</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>70</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>69</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>71</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>72</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>118</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>119</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>120</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>32</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>33</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>121</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>122</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>73</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>74</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>123</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>184</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>125</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>126</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>168</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>185</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>34</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>35</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>75</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>36</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>124</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>127</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>76</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>128</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>186</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>77</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>187</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>169</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>129</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>130</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>57</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>37</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>78</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>79</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>188</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>86</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>170</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>189</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>171</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>172</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>59</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>80</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>104</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>38</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>39</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>131</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>173</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>174</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>175</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>191</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>40</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>47</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>133</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>42</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>134</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>135</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>81</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>190</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>136</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>137</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>56</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>197</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>83</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>138</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>177</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>139</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>43</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>82</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>132</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>85</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>84</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>140</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>87</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>88</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>141</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>192</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>178</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>142</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>44</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>89</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>193</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>143</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>45</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>58</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>176</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>179</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>90</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>194</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>41</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>180</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>91</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>92</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>144</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>145</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>relgruppi</nometabella>
<colonnetabella>
<nomecolonna>idutente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idgruppo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>beniinventario</nometabella>
<colonnetabella>
<nomecolonna>idbeniinventario</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_bene</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_bene</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>descrizione_bene</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>magazzini</nometabella>
<colonnetabella>
<nomecolonna>idmagazzini</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_magazzino</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_magazzino</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>descrizione_magazzino</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numpiano</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numcasa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>relinventario</nometabella>
<colonnetabella>
<nomecolonna>idbeneinventario</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idappartamento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idmagazzino</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>quantita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>quantita_min_predef</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>richiesto_checkin</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>casse</nometabella>
<colonnetabella>
<nomecolonna>idcasse</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_cassa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>stato</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice_cassa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>descrizione_cassa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>2025-11-16 10:58:02</cmp><cmp>127.0.0.1</cmp><cmp>1</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>contratti</nometabella>
<colonnetabella>
<nomecolonna>numero</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>2322</cmp><cmp>vett9</cmp><cmp>situation_clean;unit_clean</cmp></riga>
<riga><cmp>2323</cmp><cmp>vett9</cmp><cmp>people_clean;unit_clean</cmp></riga>
<riga><cmp>2324</cmp><cmp>vett9</cmp><cmp>dep_people_clean;unit_clean</cmp></riga>
<riga><cmp>2325</cmp><cmp>vett9</cmp><cmp>people_num_clean;unit_clean</cmp></riga>
<riga><cmp>2326</cmp><cmp>vett9</cmp><cmp>dep_people_num_clean;unit_clean</cmp></riga>
<riga><cmp>2327</cmp><cmp>vett9</cmp><cmp>arr_people_num_clean;unit_clean</cmp></riga>
<riga><cmp>2328</cmp><cmp>vett9</cmp><cmp>arrival_time_clean;unit_clean</cmp></riga>
<riga><cmp>2329</cmp><cmp>vett9</cmp><cmp>extra_costs_clean;ec_num_clean</cmp></riga>
<riga><cmp>2330</cmp><cmp>vett9</cmp><cmp>ec_pos_clean;nome_costo_agg</cmp></riga>
<riga><cmp>2332</cmp><cmp>vett9</cmp><cmp>ec_unit_row_clean;unit_clean</cmp></riga>
<riga><cmp>2333</cmp><cmp>vett9</cmp><cmp>array_dates_clean;day_clean</cmp></riga>
<riga><cmp>2334</cmp><cmp>vett9</cmp><cmp>total_ec_clean;ec_num_clean</cmp></riga>
<riga><cmp>2340</cmp><cmp>vett8</cmp><cmp>arr_links_wle;numero_cliente</cmp></riga>
<riga><cmp>2341</cmp><cmp>vett8</cmp><cmp>arr_links_es_wle;numero_cliente</cmp></riga>
<riga><cmp>2336</cmp><cmp>vett7</cmp><cmp>arr_links_cre;numero_cliente</cmp></riga>
<riga><cmp>2337</cmp><cmp>vett7</cmp><cmp>arr_links_es_cre;numero_cliente</cmp></riga>
<riga><cmp>2338</cmp><cmp>vett7</cmp><cmp>arr_access_data_cre;numero_cliente</cmp></riga>
<riga><cmp>2339</cmp><cmp>vett7</cmp><cmp>arr_access_data_es_cre;numero_cliente</cmp></riga>
<riga><cmp>1</cmp><cmp>vett2</cmp><cmp>vat_perc_arr_invo;vat_num_invo</cmp></riga>
<riga><cmp>2</cmp><cmp>vett2</cmp><cmp>exist_perc_vat_invo;tmp_var_invo</cmp></riga>
<riga><cmp>2335</cmp><cmp>vett10</cmp><cmp>client_shown_csv;numero_cliente</cmp></riga>
<riga><cmp>64</cmp><cmp>var9</cmp><cmp>report_date_clean</cmp></riga>
<riga><cmp>65</cmp><cmp>var9</cmp><cmp>arrival_clean</cmp></riga>
<riga><cmp>66</cmp><cmp>var9</cmp><cmp>departure_clean</cmp></riga>
<riga><cmp>67</cmp><cmp>var9</cmp><cmp>unit_clean</cmp></riga>
<riga><cmp>68</cmp><cmp>var9</cmp><cmp>row_class_clean</cmp></riga>
<riga><cmp>69</cmp><cmp>var9</cmp><cmp>f_report_date_clean</cmp></riga>
<riga><cmp>70</cmp><cmp>var9</cmp><cmp>people_tot_clean</cmp></riga>
<riga><cmp>71</cmp><cmp>var9</cmp><cmp>dep_people_tot_clean</cmp></riga>
<riga><cmp>72</cmp><cmp>var9</cmp><cmp>arr_people_tot_clean</cmp></riga>
<riga><cmp>73</cmp><cmp>var9</cmp><cmp>ec_num_clean</cmp></riga>
<riga><cmp>74</cmp><cmp>var9</cmp><cmp>repetition_number_clean</cmp></riga>
<riga><cmp>75</cmp><cmp>var9</cmp><cmp>var_tmp_clean</cmp></riga>
<riga><cmp>76</cmp><cmp>var9</cmp><cmp>ec_table_head_clean</cmp></riga>
<riga><cmp>77</cmp><cmp>var9</cmp><cmp>ec_table_row_clean</cmp></riga>
<riga><cmp>78</cmp><cmp>var9</cmp><cmp>day_clean</cmp></riga>
<riga><cmp>79</cmp><cmp>var9</cmp><cmp>header_row_table_clean</cmp></riga>
<riga><cmp>80</cmp><cmp>var9</cmp><cmp>unit_repetition_number_clean</cmp></riga>
<riga><cmp>81</cmp><cmp>var9</cmp><cmp>repeat_header_row_clean</cmp></riga>
<riga><cmp>82</cmp><cmp>var9</cmp><cmp>number_repeat_head_row_clean</cmp></riga>
<riga><cmp>84</cmp><cmp>var8</cmp><cmp>surname_wle</cmp></riga>
<riga><cmp>85</cmp><cmp>var8</cmp><cmp>surn_no_sp_wle</cmp></riga>
<riga><cmp>20</cmp><cmp>var7</cmp><cmp>surname_cre</cmp></riga>
<riga><cmp>21</cmp><cmp>var7</cmp><cmp>surn_no_sp_cre</cmp></riga>
<riga><cmp>19</cmp><cmp>var6</cmp><cmp>surname_avail_eml</cmp></riga>
<riga><cmp>12</cmp><cmp>var4</cmp><cmp>city_row_recei</cmp></riga>
<riga><cmp>13</cmp><cmp>var4</cmp><cmp>nation_row_recei</cmp></riga>
<riga><cmp>14</cmp><cmp>var4</cmp><cmp>struct_fisc_code_recei</cmp></riga>
<riga><cmp>15</cmp><cmp>var4</cmp><cmp>first_name_recei</cmp></riga>
<riga><cmp>16</cmp><cmp>var4</cmp><cmp>surname_recei</cmp></riga>
<riga><cmp>17</cmp><cmp>var4</cmp><cmp>struct_telephone_recei</cmp></riga>
<riga><cmp>18</cmp><cmp>var4</cmp><cmp>street_num_recei</cmp></riga>
<riga><cmp>57</cmp><cmp>var4</cmp><cmp>show_method_recei</cmp></riga>
<riga><cmp>83</cmp><cmp>var4</cmp><cmp>logo_recei</cmp></riga>
<riga><cmp>22</cmp><cmp>var2</cmp><cmp>city_row_invo</cmp></riga>
<riga><cmp>23</cmp><cmp>var2</cmp><cmp>nation_row_invo</cmp></riga>
<riga><cmp>24</cmp><cmp>var2</cmp><cmp>struct_fisc_code_invo</cmp></riga>
<riga><cmp>25</cmp><cmp>var2</cmp><cmp>first_name_invo</cmp></riga>
<riga><cmp>26</cmp><cmp>var2</cmp><cmp>surname_invo</cmp></riga>
<riga><cmp>27</cmp><cmp>var2</cmp><cmp>struct_telephone_invo</cmp></riga>
<riga><cmp>28</cmp><cmp>var2</cmp><cmp>tmp_var_invo</cmp></riga>
<riga><cmp>29</cmp><cmp>var2</cmp><cmp>rate_no_vat_invo</cmp></riga>
<riga><cmp>30</cmp><cmp>var2</cmp><cmp>last_reserv_invo</cmp></riga>
<riga><cmp>31</cmp><cmp>var2</cmp><cmp>extra_cost_name_invo</cmp></riga>
<riga><cmp>32</cmp><cmp>var2</cmp><cmp>tot_no_vat_invo</cmp></riga>
<riga><cmp>33</cmp><cmp>var2</cmp><cmp>price_tot_invo</cmp></riga>
<riga><cmp>34</cmp><cmp>var2</cmp><cmp>price_tot_invo_p</cmp></riga>
<riga><cmp>35</cmp><cmp>var2</cmp><cmp>vat_invo_p</cmp></riga>
<riga><cmp>36</cmp><cmp>var2</cmp><cmp>tot_no_vat_invo_p</cmp></riga>
<riga><cmp>37</cmp><cmp>var2</cmp><cmp>extra_cost_no_vat_invo_p</cmp></riga>
<riga><cmp>38</cmp><cmp>var2</cmp><cmp>discount_no_vat_invo_p</cmp></riga>
<riga><cmp>39</cmp><cmp>var2</cmp><cmp>rate_no_vat_invo_p</cmp></riga>
<riga><cmp>40</cmp><cmp>var2</cmp><cmp>street_num_invo</cmp></riga>
<riga><cmp>41</cmp><cmp>var2</cmp><cmp>fiscal_code_invo</cmp></riga>
<riga><cmp>42</cmp><cmp>var2</cmp><cmp>vat_number_invo</cmp></riga>
<riga><cmp>43</cmp><cmp>var2</cmp><cmp>street_invo</cmp></riga>
<riga><cmp>44</cmp><cmp>var2</cmp><cmp>vat_num_invo</cmp></riga>
<riga><cmp>45</cmp><cmp>var2</cmp><cmp>show_rate_invo</cmp></riga>
<riga><cmp>46</cmp><cmp>var2</cmp><cmp>show_discount_invo</cmp></riga>
<riga><cmp>47</cmp><cmp>var2</cmp><cmp>show_extra_cost_invo</cmp></riga>
<riga><cmp>48</cmp><cmp>var2</cmp><cmp>repetition_num_invo</cmp></riga>
<riga><cmp>49</cmp><cmp>var2</cmp><cmp>part_tot_no_vat_invo</cmp></riga>
<riga><cmp>50</cmp><cmp>var2</cmp><cmp>part_tot_vat_invo</cmp></riga>
<riga><cmp>51</cmp><cmp>var2</cmp><cmp>part_tot_no_vat_invo_p</cmp></riga>
<riga><cmp>52</cmp><cmp>var2</cmp><cmp>part_tot_vat_invo_p</cmp></riga>
<riga><cmp>53</cmp><cmp>var2</cmp><cmp>max_vat_num_invo</cmp></riga>
<riga><cmp>54</cmp><cmp>var2</cmp><cmp>people_phrase_invo</cmp></riga>
<riga><cmp>55</cmp><cmp>var2</cmp><cmp>merge_discount_with_rate</cmp></riga>
<riga><cmp>56</cmp><cmp>var2</cmp><cmp>logo_invo</cmp></riga>
<riga><cmp>58</cmp><cmp>var2</cmp><cmp>tax_cost_name_invo</cmp></riga>
<riga><cmp>59</cmp><cmp>var2</cmp><cmp>show_tax_cost_invo</cmp></riga>
<riga><cmp>60</cmp><cmp>var2</cmp><cmp>vat_invo</cmp></riga>
<riga><cmp>61</cmp><cmp>var2</cmp><cmp>tot_costs_tax_invo</cmp></riga>
<riga><cmp>62</cmp><cmp>var2</cmp><cmp>show_cost_as_taxes_invo</cmp></riga>
<riga><cmp>63</cmp><cmp>var2</cmp><cmp>show_subtotal_invo</cmp></riga>
<riga><cmp>119</cmp><cmp>var2</cmp><cmp>round_on_totals_invo</cmp></riga>
<riga><cmp>120</cmp><cmp>var2</cmp><cmp>part_price_tot_invo</cmp></riga>
<riga><cmp>121</cmp><cmp>var2</cmp><cmp>rate_taxes_perc_invo</cmp></riga>
<riga><cmp>106</cmp><cmp>var11</cmp><cmp>surname_rcsv</cmp></riga>
<riga><cmp>107</cmp><cmp>var11</cmp><cmp>name_rcsv</cmp></riga>
<riga><cmp>108</cmp><cmp>var11</cmp><cmp>unit_rcsv</cmp></riga>
<riga><cmp>109</cmp><cmp>var11</cmp><cmp>rate_name_rcsv</cmp></riga>
<riga><cmp>110</cmp><cmp>var11</cmp><cmp>email_rcsv</cmp></riga>
<riga><cmp>111</cmp><cmp>var11</cmp><cmp>telephone_rcsv</cmp></riga>
<riga><cmp>112</cmp><cmp>var11</cmp><cmp>rate_price_rcsv</cmp></riga>
<riga><cmp>113</cmp><cmp>var11</cmp><cmp>total_price_rcsv</cmp></riga>
<riga><cmp>114</cmp><cmp>var11</cmp><cmp>paid_rcsv</cmp></riga>
<riga><cmp>115</cmp><cmp>var11</cmp><cmp>total_people_rcsv</cmp></riga>
<riga><cmp>116</cmp><cmp>var11</cmp><cmp>comment_rcsv</cmp></riga>
<riga><cmp>117</cmp><cmp>var11</cmp><cmp>arrival_rcsv</cmp></riga>
<riga><cmp>118</cmp><cmp>var11</cmp><cmp>departure_rcsv</cmp></riga>
<riga><cmp>86</cmp><cmp>var10</cmp><cmp>surname_csv</cmp></riga>
<riga><cmp>87</cmp><cmp>var10</cmp><cmp>name_csv</cmp></riga>
<riga><cmp>88</cmp><cmp>var10</cmp><cmp>nickname_csv</cmp></riga>
<riga><cmp>89</cmp><cmp>var10</cmp><cmp>title_csv</cmp></riga>
<riga><cmp>90</cmp><cmp>var10</cmp><cmp>email_csv</cmp></riga>
<riga><cmp>91</cmp><cmp>var10</cmp><cmp>telephone_csv</cmp></riga>
<riga><cmp>92</cmp><cmp>var10</cmp><cmp>fax_csv</cmp></riga>
<riga><cmp>93</cmp><cmp>var10</cmp><cmp>nation_csv</cmp></riga>
<riga><cmp>94</cmp><cmp>var10</cmp><cmp>region_csv</cmp></riga>
<riga><cmp>95</cmp><cmp>var10</cmp><cmp>city_csv</cmp></riga>
<riga><cmp>96</cmp><cmp>var10</cmp><cmp>address_csv</cmp></riga>
<riga><cmp>97</cmp><cmp>var10</cmp><cmp>postal_code_csv</cmp></riga>
<riga><cmp>98</cmp><cmp>var10</cmp><cmp>nationality_csv</cmp></riga>
<riga><cmp>99</cmp><cmp>var10</cmp><cmp>birthdate_csv</cmp></riga>
<riga><cmp>100</cmp><cmp>var10</cmp><cmp>vat_number_csv</cmp></riga>
<riga><cmp>101</cmp><cmp>var10</cmp><cmp>tmp_csv</cmp></riga>
<riga><cmp>102</cmp><cmp>var10</cmp><cmp>email2_csv</cmp></riga>
<riga><cmp>103</cmp><cmp>var10</cmp><cmp>certified_email_csv</cmp></riga>
<riga><cmp>104</cmp><cmp>var10</cmp><cmp>telephone2_csv</cmp></riga>
<riga><cmp>105</cmp><cmp>var10</cmp><cmp>telephone3_csv</cmp></riga>
<riga><cmp>1</cmp><cmp>var</cmp><cmp>Mr</cmp></riga>
<riga><cmp>2</cmp><cmp>var</cmp><cmp>il</cmp></riga>
<riga><cmp>3</cmp><cmp>var</cmp><cmp>Il_</cmp></riga>
<riga><cmp>4</cmp><cmp>var</cmp><cmp>al</cmp></riga>
<riga><cmp>5</cmp><cmp>var</cmp><cmp>e</cmp></riga>
<riga><cmp>6</cmp><cmp>var</cmp><cmp>o</cmp></riga>
<riga><cmp>7</cmp><cmp>var</cmp><cmp>el</cmp></riga>
<riga><cmp>8</cmp><cmp>var</cmp><cmp>El_</cmp></riga>
<riga><cmp>9</cmp><cmp>var</cmp><cmp>al3</cmp></riga>
<riga><cmp>10</cmp><cmp>var</cmp><cmp>a</cmp></riga>
<riga><cmp>11</cmp><cmp>var</cmp><cmp>o3</cmp></riga>
<riga><cmp>6</cmp><cmp>opzeml</cmp><cmp>;;</cmp></riga>
<riga><cmp>7</cmp><cmp>opzeml</cmp><cmp>;SI;</cmp></riga>
<riga><cmp>8</cmp><cmp>opzeml</cmp><cmp>;SI;</cmp></riga>
<riga><cmp>6</cmp><cmp>oggetto</cmp><cmp>mln_en:Availability>mln_es:Disponibilidad</cmp></riga>
<riga><cmp>7</cmp><cmp>oggetto</cmp><cmp>mln_en:Reservation confirmation>mln_es:Confirmación reserva</cmp></riga>
<riga><cmp>8</cmp><cmp>oggetto</cmp><cmp>mln_en:Your reservation is approaching>mln_es:Se acerca tu reserva</cmp></riga>
<riga><cmp>3</cmp><cmp>nomefile</cmp><cmp>Invoice</cmp></riga>
<riga><cmp>6</cmp><cmp>mln_es</cmp><cmp>Estimad[o] Señor[a] [surname_avail_eml],
le confirmo la disponibilidad de un apartamento[c num_personas_tot!=""] para [num_personas_tot] personas[/c] para el período desde el [fecha_inicial] hasta el [fecha_final]. El precio para dicho período es de [coste_tot_p] [nombre_divisa] (incluyendo costes asociados).

En el caso de que desee reservar le ruego me envie su confirmación respondiendo a este correo electrónico.

Estaré a su disposición para cualquier otra información que necesite.

Saludos,
[nombre_contacto_estructura]

[nombre_estructura]
[sitio_web_estructura]


[texto_citado_email_pedido]
</cmp></riga>
<riga><cmp>7</cmp><cmp>mln_es</cmp><cmp>Estimad[o] Señor[a] [surname_cre],
le confirmo que he reservado a su nombre un apartamento[c num_personas_tot!=""] para [num_personas_tot] personas[/c] para el período desde el [fecha_inicial] hasta el [fecha_final]. El precio para dicho período es de [coste_tot_p] [nombre_divisa] (incluyendo costes asociados). Para completar la reserva es necesario pagar por adelantado [fianza_p] [nombre_divisa], puede efectuar este pago siguiendo [c arr_links_cre(client_number)=""]este enlace[/c][c arr_links_cre(client_number)!=""]estos enlaces[/c]:

[arr_links_es_cre(client_number)][base_url_for_webpages]mdl_confirma_reserva.php?cn=[surn_no_sp_cre]&cp=[reservation_code]

Si [c arr_links_cre(client_number)=""]el enlace no funcionara[/c][c arr_links_cre(client_number)!=""]los enlaces no funcionaran[/c] correctamente puede intentar utilizar este otro:

[base_url_for_webpages]mdl_confirma_reserva.php

e insertar después:

[arr_access_data_es_cre(client_number)]Apellido: [apellido]
Código reserva: [codigo_reserva]

Estaré a su disposición para cualquier otra información que necesite.

Saludos,
[nombre_contacto_estructura]

[nombre_estructura]
[sitio_web_estructura]
</cmp></riga>
<riga><cmp>8</cmp><cmp>mln_es</cmp><cmp>Estimad[o] Señor[a] [surname_wle],
[c attachment1!=""]he adjuntado a este correo electrónico un archivo con nuestros contactos y un mapa para ayudarles a encontrarnos, avíseme si tiene problemas para leerlo.

[/c]Si desea ahorrar tiempo a su llegada, puede completar los datos necesarios para registrarse desde aquí:

[arr_links_es_wle(client_number)][base_url_for_webpages]mdl_confirma_reserva.php?cn=[surn_no_sp_wle]&cp=[reservation_code]&fe=1

[c estimated_checkin_time=""]¿Conoce su hora estimada de llegada? ¡Gracias! [/c]Estaré a su disposición para cualquier otra información que necesite.

Cordiales saludos,
[nombre_contacto_estructura]

[nombre_estructura]
[sitio_web_estructura]
</cmp></riga>
<riga><cmp>6</cmp><cmp>mln_en</cmp><cmp>Dear Mr[Mr] [surname_avail_eml],
I confirm you the availability of an apartment[c people_num_tot!=""] for [people_num_tot] people[/c] in the period from [starting_date] to [ending_date]. The price for this period is [price_tot_p] [currency_name] (including cleaning and utilities).

If you are interested in reserving the apartment you can contact me by replaying to this email.

Please let me know if you have any question.

Best regards,
[structure_contact_name]

[structure_name]
[structure_website]


[enquiry_email_quoted_text]
</cmp></riga>
<riga><cmp>7</cmp><cmp>mln_en</cmp><cmp>Dear Mr[Mr] [surname_cre],
I confirm you that I have reserved you an apartment[c people_num_tot!=""] for [people_num_tot] people[/c] in the period from [starting_date] to [ending_date]. The price for this period is [price_tot_p] [currency_name] (including cleaning and utilities). In order to complete the reservation you must send a down-payment of [deposit_p] [currency_name], you can pay it following [c arr_links_cre(client_number)=""]this link[/c][c arr_links_cre(client_number)!=""]these links[/c]:

[arr_links_cre(client_number)][base_url_for_webpages]confirm_reservation_tpl.php?cn=[surn_no_sp_cre]&cp=[reservation_code]

If the above [c arr_links_cre(client_number)=""]link does not[/c][c arr_links_cre(client_number)!=""]links do not[/c] work properly for you, try this other one:

[base_url_for_webpages]confirm_reservation_tpl.php

and then insert:

[arr_access_data_cre(client_number)]Surname: [surname]
Reservation code: [reservation_code]

Please let me know if you have any other question.

Best regards,
[structure_contact_name]

[structure_name]
[structure_website]
</cmp></riga>
<riga><cmp>8</cmp><cmp>mln_en</cmp><cmp>Dear Mr[Mr] [surname_wle],
[c attachment1!=""]I’ve attached to this e-mail a file with our contacts and a map to help you find us, please let me know if you have problems reading it.

[/c]If you want to save time at your arrival you can fill in the data required for check-in from here:

[arr_links_wle(client_number)][base_url_for_webpages]confirm_reservation_tpl.php?cn=[surn_no_sp_wle]&cp=[reservation_code]&fe=1

[c estimated_checkin_time=""]Do you know your estimated time of arrival? Thanks! [/c]Please let me know if you have any other question.

Best regards,
[structure_contact_name]

[structure_name]
[structure_website]
</cmp></riga>
<riga><cmp>3</cmp><cmp>impor_vc</cmp><cmp>2</cmp></riga>
<riga><cmp>5</cmp><cmp>impor_vc</cmp><cmp>4</cmp></riga>
<riga><cmp>9</cmp><cmp>headhtm</cmp><cmp><!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" >
<title>Cleaning report</title>
<style type="text/css">
table.clrep { border-collapse: collapse; }
table.clrep td { border: 2px solid black; padding: 2px; text-align: center; }
.headrow { background-color: #cccccc; }
tr.bgclr { background-color: #eeeeee; }
</style>
</head>
<body style="background-color: #ffffff;">
</cmp></riga>
<riga><cmp>9</cmp><cmp>foothtm</cmp><cmp></body>
</html>
</cmp></riga>
<riga><cmp>2</cmp><cmp>dir</cmp><cmp>~</cmp></riga>
<riga><cmp>3</cmp><cmp>dir</cmp><cmp>~</cmp></riga>
<riga><cmp>10</cmp><cmp>datdownl</cmp><cmp>csv></cmp></riga>
<riga><cmp>11</cmp><cmp>datdownl</cmp><cmp>csv></cmp></riga>
<riga><cmp>10</cmp><cmp>contrtxt</cmp><cmp>Surname,Name,Nickname,Title,Sex,Email,2nd Email,Certified Email,Telephone,2nd Telephone,3rd Telephone,Fax,Language,Nation of Residency,Region of Residency,City of Residency,Address,Postal Code,Nationality,Date of Birth,Vat Number
[r][surname_csv],[name_csv],[nickname_csv],[title_csv],[sex],[email_csv],[email2_csv],[certified_email_csv],[telephone_csv],[telephone2_csv],[telephone3_csv],[fax_csv],[language_code],[nation_csv],[region_csv],[city_csv],[address_csv],[postal_code_csv],[nationality_csv],[birthdate_csv],[vat_number_csv]
[/r]</cmp></riga>
<riga><cmp>11</cmp><cmp>contrtxt</cmp><cmp>Arrival,Departure,Surname,Name,Email,Telephone,Total People,Occupied Unit,Rate Name,Rate Price,Total Price,Paid,Comment
[r][arrival_rcsv],[departure_rcsv],[surname_rcsv],[name_rcsv],[email_rcsv],[telephone_rcsv],[total_people_rcsv],[unit_rcsv],[rate_name_rcsv],[rate_price_rcsv],[total_price_rcsv],[paid_rcsv],[comment_rcsv]
[/r]</cmp></riga>
<riga><cmp>3</cmp><cmp>contrrtf</cmp><cmp>{\rtf1\ansi\deff1\adeflang1025[r][r3][/r3] [/r]
{\fonttbl{\f0\froman\fprq2\fcharset0 Times New Roman;}{\f1\froman\fprq2\fcharset0 Times New Roman;}{\f2\fswiss\fprq2\fcharset0 Arial;}{\f3\fswiss\fprq2\fcharset0 Arial;}{\f4\fswiss\fprq2\fcharset0 Bitstream Vera Sans;}{\f5\fswiss\fprq2\fcharset0 Tahoma;}{\f6\froman\fprq2\fcharset0 Garamond;}{\f7\froman\fprq2\fcharset0 Times New Roman;}{\f8\fnil\fprq2\fcharset0 Bitstream Vera Sans;}}
{\colortbl;\red0\green0\blue0;\red230\green230\blue230;\red255\green255\blue255;\red204\green204\blue204;\red128\green128\blue128;}
{\stylesheet{\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\snext1 Normal;}
{\s2\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af8\afs28\lang255\ltrch\dbch\af8\langfe255\hich\f2\fs28\lang1040\loch\f2\fs28\lang1040\sbasedon1\snext3 Heading;}
{\s3\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af3\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon1\snext3 Body Text;}
{\s4{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon3\snext4 List;}
{\s5\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext5 caption;}
{\s6{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext6 Index;}
{\s7\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 Heading;}
{\s8\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext8 caption;}
{\s9{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext9 Index;}
{\s10\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading;}
{\s11\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext11 WW-caption;}
{\s12{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext12 WW-Index;}
{\s13\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading1;}
{\s14\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext14 WW-caption1;}
{\s15{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext15 WW-Index1;}
{\s16\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading11;}
{\s17\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext17 WW-caption11;}
{\s18{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext18 WW-Index11;}
{\s19\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading111;}
{\s20\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext20 WW-caption111;}
{\s21{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext21 WW-Index111;}
{\s22\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading1111;}
{\s23\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext23 WW-caption1111;}
{\s24{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext24 WW-Index1111;}
{\s25\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading11111;}
{\s26\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext26 WW-caption11111;}
{\s27{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext27 WW-Index11111;}
{\s28\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading111111;}
{\s29\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext29 WW-caption111111;}
{\s30{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext30 WW-Index111111;}
{\s31\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading1111111;}
{\s32\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext32 WW-caption1111111;}
{\s33{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext33 WW-Index1111111;}
{\s34\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af4\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f4\fs28\lang1040\loch\f4\fs28\lang1040\sbasedon1\snext3 WW-Heading11111111;}
{\s35\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext35 WW-caption11111111;}
{\s36{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af3\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon1\snext36 WW-Index11111111;}
{\s37\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs20\lang255\ai\ltrch\dbch\af3\langfe255\hich\f1\fs20\lang1033\i\loch\f1\fs20\lang1033\i\sbasedon1\snext37 Dicitura;}
{\s38{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af5\afs16\lang255\ltrch\dbch\af3\langfe255\hich\f5\fs16\lang1033\loch\f5\fs16\lang1033\sbasedon1\snext38 WW-Testo fumetto;}
{\s39{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon3\snext39 Frame contents;}
{\s40{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon3\snext40 Table Contents;}
{\s41\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ab\ltrch\dbch\langfe255\hich\f1\fs24\lang1033\i\b\loch\f1\fs24\lang1033\i\b\sbasedon40\snext41 Table Heading;}
{\s42{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext42 WW-Table Contents;}
{\s43\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon42\snext43 WW-Table Heading;}
{\s44{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext44 WW-Table Contents1;}
{\s45\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon44\snext45 WW-Table Heading1;}
{\s46{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext46 WW-Table Contents12;}
{\s47\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon46\snext47 WW-Table Heading12;}
{\s48{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext48 WW-Table Contents123;}
{\s49\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon48\snext49 WW-Table Heading123;}
{\s50{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext50 WW-Table Contents1234;}
{\s51\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon50\snext51 WW-Table Heading1234;}
{\s52{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext52 WW-Table Contents12345;}
{\s53\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon52\snext53 WW-Table Heading12345;}
{\s54{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext54 WW-Table Contents123456;}
{\s55\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon54\snext55 WW-Table Heading123456;}
{\s56{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext56 WW-Table Contents1234567;}
{\s57\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon56\snext57 WW-Table Heading1234567;}
{\s58{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext58 WW-Table Contents12345678;}
{\s59\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon58\snext59 WW-Table Heading12345678;}
{\s60{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext60 Table Contents;}
{\s61\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon60\snext61 Table Heading;}
{\*\cs63\cf0\rtlch\af1\afs24\lang255\ltrch\dbch\af3\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 WW-Car. predefinito paragrafo;}
}
{\info{\creatim\yr2007\mo9\dy28\hr15\min45}{\revtim\yr1601\mo1\dy1\hr0\min0}{\printim\yr1601\mo1\dy1\hr0\min0}{\comment StarWriter}{\vern3000}}\deftab708
{\*\pgdsctbl
{\pgdsc0\pgdscuse195\pgwsxn11905\pghsxn16837\marglsxn1134\margrsxn1134\margtsxn885\margbsxn1012\pgdscnxt0 Standard;}}
{\*\pgdscno0}\paperh16837\paperw11905\margl1134\margr1134\margt885\margb1012\sectd\sbknone\pgwsxn11905\pghsxn16837\marglsxn1134\margrsxn1134\margtsxn885\margbsxn1012\ftnbj\ftnstart1\ftnrstcont\ftnnar\aenddoc\aftnrstcont\aftnstart1\aftnnrlc
\pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs28\lang255\ab\ltrch\dbch\af1\langfe255\hich\f6\fs28\lang1040\b\loch\f6\fs28\lang1040\b {\rtlch \ltrch\loch\f6\fs28\lang1040\i0\b [structure_type] [structure_name]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [structure_company_name]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [structure_address] - [structure_city]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [structure_postal_code] [structure_nation]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 VAT number [structure_vat_number] [struct_fisc_code_invo]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [struct_telephone_invo]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\li5370\ri0\lin5370\rin0\fi0\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 Invoice for [first_name_invo] [surname_invo] }
[c street_invo!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab [street_invo][street_num_invo]}
[/c][c city_row_invo!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab [city_row_invo]}
[/c][c nation_row_invo!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab [nation_row_invo]}
[/c][c fiscal_code_invo!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab Fiscal code [fiscal_code_invo]}
[/c][c vat_number_invo!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab VAT number [vat_number_invo]}
[/c]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\brdrb\brdrs\brdrw20\brdrcf1\brsp20{\*\brdrb\brdlncol1\brdlnin0\brdlnout20\brdlndist0}\brsp20\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1\tx3540{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 \tab }
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 Invoice n. [document_progressive_number] released on [today]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \trowd\trql\trleft276\trrh-119\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrb\brdrs\brdrw1\brdrcf1\cellx7792\clbrdrb\brdrs\brdrw1\brdrcf1\clvertalb\cellx9637
[r4 array="vat_perc_arr_invo"]
\pard\intbl\pard\plain \intbl\ltrpar\s1\cf0\cbpat3\ql\rtlch\afs12\lang255\ltrch\dbch\langfe255\hich\fs12\lang1040\loch\fs12\lang1040 
\cell\pard\plain \intbl\ltrpar\s1\cf0\ql\rtlch\afs24\lang255\ltrch\dbch\langfe255\hich\fs24\lang1040\loch\fs24\lang1040 
[r]
[c show_rate_invo="1"]\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Stay from [starting_date] to [ending_date][people_phrase_invo]}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [rate_no_vat_invo_p]}
[/c][c show_discount_invo="1"]\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Discount}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [discount_no_vat_invo_p]}
[/c]
[r3][c show_extra_cost_invo="1"]\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Extra: \'93[extra_cost_name]\'94}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [extra_cost_no_vat_invo_p]}
[/c][c show_cost_as_taxes_invo="1"]\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Tax: \'93[extra_cost_name]\'94}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [extra_cost_taxes_p]}
[/c][/r3][/r]
[c show_subtotal_invo="1"]\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Sub total at [vat_perc_arr_invo(vat_num_invo)]%}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [part_tot_no_vat_invo_p]}
\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Taxes at [vat_perc_arr_invo(vat_num_invo)]%}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [part_tot_vat_invo_p]}
[/c]\cell\row\pard \trowd\trql\trleft276\trrh-119\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat3\cellx7792\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat3\clvertalb\cellx9637
[/r4]
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs12\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs12\lang1040\loch\f1\fs12\lang1040 
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Sub total}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [tot_no_vat_invo_p]}
\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Taxes[c vat_num_invo="1"] at [vat_perc_arr_invo(vat_num_invo)]%[/c] total}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [vat_invo_p]}
[r][r3][c show_tax_cost_invo="1"]
\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [extra_cost_name]}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [currency_name] [extra_cost_no_vat_invo_p]}
[/c][/r3][/r]
\cell\row\pard \trowd\trql\trleft276\trrh-119\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat3\cellx7792\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat3\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs12\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs12\lang1040\loch\f1\fs12\lang1040 
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat4\cellx7792\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat4\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1\cf0\ql\rtlch\afs24\lang255\ltrch\dbch\langfe255\hich\fs24\lang1040\loch\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Invoice total}
\cell\pard\plain \intbl\ltrpar\s1\cf0\qr\rtlch\afs24\lang255\ab\ltrch\dbch\langfe255\hich\fs24\lang1040\b\loch\fs24\lang1040\b {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b [currency_name] [price_tot_invo_p]}
\cell\row\pard \pard\plain \ltrpar\s1\cf0\ql\rtlch\afs24\lang255\ltrch\dbch\langfe255\hich\fs24\lang1040\loch\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\brdrb\brdrs\brdrw20\brdrcf1\brsp20{\*\brdrb\brdlncol1\brdlnin0\brdlnout20\brdlndist0}\brsp20\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par }</cmp></riga>
<riga><cmp>5</cmp><cmp>contrrtf</cmp><cmp>{\rtf1\ansi\deff1\adeflang1025
{\fonttbl{\f0\froman\fprq2\fcharset0 Times New Roman;}{\f1\froman\fprq2\fcharset0 Times New Roman;}{\f2\fswiss\fprq2\fcharset0 Arial;}{\f3\fswiss\fprq2\fcharset0 Arial;}{\f4\fswiss\fprq2\fcharset0 Bitstream Vera Sans;}{\f5\fswiss\fprq2\fcharset0 Tahoma;}{\f6\froman\fprq2\fcharset0 Garamond;}{\f7\froman\fprq2\fcharset0 Times New Roman;}{\f8\fnil\fprq2\fcharset0 Bitstream Vera Sans;}}
{\colortbl;\red0\green0\blue0;\red230\green230\blue230;\red255\green255\blue255;\red204\green204\blue204;\red128\green128\blue128;}
{\stylesheet{\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\snext1 Normal;}
{\s2\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af8\afs28\lang255\ltrch\dbch\af8\langfe255\hich\f2\fs28\lang1040\loch\f2\fs28\lang1040\sbasedon1\snext3 Heading;}
{\s3\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af3\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon1\snext3 Body Text;}
{\s4{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon3\snext4 List;}
{\s5\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext5 caption;}
{\s6{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext6 Index;}
{\s7\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 Heading;}
{\s8\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext8 caption;}
{\s9{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext9 Index;}
{\s10\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading;}
{\s11\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext11 WW-caption;}
{\s12{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext12 WW-Index;}
{\s13\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading1;}
{\s14\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext14 WW-caption1;}
{\s15{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext15 WW-Index1;}
{\s16\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading11;}
{\s17\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext17 WW-caption11;}
{\s18{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext18 WW-Index11;}
{\s19\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading111;}
{\s20\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext20 WW-caption111;}
{\s21{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext21 WW-Index111;}
{\s22\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading1111;}
{\s23\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext23 WW-caption1111;}
{\s24{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext24 WW-Index1111;}
{\s25\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading11111;}
{\s26\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext26 WW-caption11111;}
{\s27{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext27 WW-Index11111;}
{\s28\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading111111;}
{\s29\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext29 WW-caption111111;}
{\s30{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext30 WW-Index111111;}
{\s31\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af3\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f3\fs28\lang1040\loch\f3\fs28\lang1040\sbasedon1\snext3 WW-Heading1111111;}
{\s32\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext32 WW-caption1111111;}
{\s33{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext33 WW-Index1111111;}
{\s34\sb240\sa120\keepn{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af4\afs28\lang255\ltrch\dbch\af4\langfe255\hich\f4\fs28\lang1040\loch\f4\fs28\lang1040\sbasedon1\snext3 WW-Heading11111111;}
{\s35\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\i\loch\f1\fs24\lang1040\i\sbasedon1\snext35 WW-caption11111111;}
{\s36{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af3\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon1\snext36 WW-Index11111111;}
{\s37\sb120\sa120{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs20\lang255\ai\ltrch\dbch\af3\langfe255\hich\f1\fs20\lang1033\i\loch\f1\fs20\lang1033\i\sbasedon1\snext37 Dicitura;}
{\s38{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af5\afs16\lang255\ltrch\dbch\af3\langfe255\hich\f5\fs16\lang1033\loch\f5\fs16\lang1033\sbasedon1\snext38 WW-Testo fumetto;}
{\s39{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon3\snext39 Frame contents;}
{\s40{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1033\loch\f1\fs24\lang1033\sbasedon3\snext40 Table Contents;}
{\s41\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ai\ab\ltrch\dbch\langfe255\hich\f1\fs24\lang1033\i\b\loch\f1\fs24\lang1033\i\b\sbasedon40\snext41 Table Heading;}
{\s42{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext42 WW-Table Contents;}
{\s43\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon42\snext43 WW-Table Heading;}
{\s44{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext44 WW-Table Contents1;}
{\s45\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon44\snext45 WW-Table Heading1;}
{\s46{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext46 WW-Table Contents12;}
{\s47\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon46\snext47 WW-Table Heading12;}
{\s48{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext48 WW-Table Contents123;}
{\s49\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon48\snext49 WW-Table Heading123;}
{\s50{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext50 WW-Table Contents1234;}
{\s51\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon50\snext51 WW-Table Heading1234;}
{\s52{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext52 WW-Table Contents12345;}
{\s53\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon52\snext53 WW-Table Heading12345;}
{\s54{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext54 WW-Table Contents123456;}
{\s55\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon54\snext55 WW-Table Heading123456;}
{\s56{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext56 WW-Table Contents1234567;}
{\s57\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon56\snext57 WW-Table Heading1234567;}
{\s58{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext58 WW-Table Contents12345678;}
{\s59\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon58\snext59 WW-Table Heading12345678;}
{\s60{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040\sbasedon1\snext60 Table Contents;}
{\s61\qc{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ab\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\b\loch\f1\fs24\lang1040\b\sbasedon60\snext61 Table Heading;}
{\*\cs63\cf0\rtlch\af1\afs24\lang255\ltrch\dbch\af3\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 WW-Car. predefinito paragrafo;}
}
{\info{\creatim\yr2007\mo9\dy28\hr15\min45}{\revtim\yr1601\mo1\dy1\hr0\min0}{\printim\yr1601\mo1\dy1\hr0\min0}{\comment StarWriter}{\vern3000}}\deftab708
{\*\pgdsctbl
{\pgdsc0\pgdscuse195\pgwsxn11905\pghsxn16837\marglsxn1134\margrsxn1134\margtsxn885\margbsxn1012\pgdscnxt0 Standard;}}
{\*\pgdscno0}\paperh16837\paperw11905\margl1134\margr1134\margt885\margb1012\sectd\sbknone\pgwsxn11905\pghsxn16837\marglsxn1134\margrsxn1134\margtsxn885\margbsxn1012\ftnbj\ftnstart1\ftnrstcont\ftnnar\aenddoc\aftnrstcont\aftnstart1\aftnnrlc
[r][c numero_ripetizione_prenotazioni!="1"]\par \page [/c]\pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs28\lang255\ab\ltrch\dbch\af1\langfe255\hich\f6\fs28\lang1040\b\loch\f6\fs28\lang1040\b {\rtlch \ltrch\loch\f6\fs28\lang1040\i0\b [structure_type] [structure_name]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [structure_company_name]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [structure_address] - [structure_city]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [structure_postal_code] [structure_nation]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 VAT number [structure_vat_number] [struct_fisc_code_recei]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [struct_telephone_recei]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\li5370\ri0\lin5370\rin0\fi0\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 [c surname_recei!=""]Receipt for [first_name_recei] [surname_recei][/c] }
[c street!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab [street][street_num_recei]}
[/c][c city_row_recei!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab [city_row_recei]}
[/c][c nation_row_recei!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab [nation_row_recei]}
[/c][c fiscal_code!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab Fiscal code [fiscal_code]}
[/c][c vat_number!=""]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 \tab \tab \tab \tab \tab \tab \tab \tab VAT number [vat_number]}
[/c]\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\brdrb\brdrs\brdrw20\brdrcf1\brsp20{\*\brdrb\brdlncol1\brdlnin0\brdlnout20\brdlndist0}\brsp20\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1\tx3540{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 \tab }
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af6\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f6\fs24\lang1040\loch\f6\fs24\lang1040 {\rtlch \ltrch\loch\f6\fs24\lang1040\i0\b0 Receipt[c progressive_document_number!=""] n. [progressive_document_number][/c] released on [today]}
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \trowd\trql\trleft276\trrh-119\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrb\brdrs\brdrw1\brdrcf1\cellx7792\clbrdrb\brdrs\brdrw1\brdrcf1\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1\cf0\cbpat3\ql\rtlch\afs12\lang255\ltrch\dbch\langfe255\hich\fs12\lang1040\loch\fs12\lang1040 
\cell\pard\plain \intbl\ltrpar\s1\cf0\ql\rtlch\afs24\lang255\ltrch\dbch\langfe255\hich\fs24\lang1040\loch\fs24\lang1040 
\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat2\cellx7792\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat2\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [c starting_date!=""]Reservation from [starting_date] to [ending_date][/c][c people_num_tot!=""] for [people_num_tot] persons[/c][r5][c starting_date="" & last_payment="1"][payment_method][/c][/r5]}
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 [r5][c show_method_recei="1" & last_payment="1"][currency_name] [payment_value_p][/c][/r5]}
\cell\row\pard \trowd\trql\trleft276\trrh-119\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat3\cellx7792\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clcbpat3\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs12\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs12\lang1040\loch\f1\fs12\lang1040 
\cell\pard\plain \intbl\ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\qr\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\cell\row\pard \trowd\trql\trleft276\trpaddft3\trpaddt55\trpaddfl3\trpaddl55\trpaddfb3\trpaddb55\trpaddfr3\trpaddr55\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrl\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat4\cellx7792\clbrdrt\brdrs\brdrw1\brdrcf1\clbrdrb\brdrs\brdrw1\brdrcf1\clbrdrr\brdrs\brdrw1\brdrcf1\clcbpat4\clvertalb\cellx9637
\pard\intbl\pard\plain \intbl\ltrpar\s1\cf0\ql\rtlch\afs24\lang255\ltrch\dbch\langfe255\hich\fs24\lang1040\loch\fs24\lang1040 {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b0 Total paid}
\cell\pard\plain \intbl\ltrpar\s1\cf0\qr\rtlch\afs24\lang255\ab\ltrch\dbch\langfe255\hich\fs24\lang1040\b\loch\fs24\lang1040\b {\rtlch \ltrch\loch\f1\fs24\lang1040\i0\b [currency_name] [r5][c last_payment="1"][payment_value_p][/c][/r5]}
\cell\row\pard \pard\plain \ltrpar\s1\cf0\ql\rtlch\afs24\lang255\ltrch\dbch\langfe255\hich\fs24\lang1040\loch\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\brdrb\brdrs\brdrw20\brdrcf1\brsp20{\*\brdrb\brdlncol1\brdlnin0\brdlnout20\brdlndist0}\brsp20\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\ql\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
\par \pard\plain \ltrpar\s1{\*\hyphen2\hyphlead2\hyphtrail2\hyphmax0}\rtlch\af1\afs24\lang255\ltrch\dbch\af1\langfe255\hich\f1\fs24\lang1040\loch\f1\fs24\lang1040 
[/r]\par }</cmp></riga>
<riga><cmp>1</cmp><cmp>contrhtm</cmp><cmp><B><U><FONT FACE="Times" SIZE=4><P ALIGN="CENTER">EXAMPLE OF CONTRACT FOR HOTELDRUID</P>
</U></B></FONT><FONT FACE="Times"><P ALIGN="JUSTIFY"></P>
<P ALIGN="JUSTIFY">&nbsp;</P><P ALIGN="JUSTIFY">
Mr[Mr] [name] [surname] born the [birthday] resident in [city] [street2] n 
[street_number] tel [telephone] will rent an apartment in hoteldruid
with his family of [people_num_tot] people from [starting_date] to [ending_date].
The price will be of [price_tot_p]. A deposit of [deposit_p] must be left.
</P>
<P ALIGN="JUSTIFY">
Nowhere, [oggi].
</P>
<P ALIGN="JUSTIFY"></P>
<P ALIGN="JUSTIFY">
The client
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
The owner</P>
<P ALIGN="JUSTIFY"></P>
<P ALIGN="JUSTIFY"> 
__________&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;____________</P>
<P ALIGN="JUSTIFY"></P>
</FONT></cmp></riga>
<riga><cmp>2</cmp><cmp>contrhtm</cmp><cmp><div style="page-break-after: always;"><br>
<div class="invoice_frame" style="width: 780px; margin-left: 10px; margin-right: 10px; border: solid 1px black; padding: 30px; font-size: 12px;">
[r][r3][/r3] [/r]

<div class="structure_data">
[logo_invo]
<div class="structure_name" style="font-size: large;">[structure_type] [structure_name]</div>
[structure_company_name]<br>
[structure_address] - [structure_city]<br>
[structure_postal_code] [structure_nation]<br>
VAT number [structure_vat_number] [struct_fisc_code_invo]<br>
[struct_telephone_invo]<br>
</div>

<div class="client_data" style="padding: 18px 0 8px 440px;">
Invoice for [first_name_invo] [surname_invo]<br>
[c street_invo!=""][street_invo][street_num_invo]<br>
[/c][c city_row_invo!=""][city_row_invo]<br>
[/c][c nation_row_invo!=""][nation_row_invo]<br>
[/c][c fiscal_code_invo!=""]Fiscal code [fiscal_code_invo]<br>
[/c][c vat_number_invo!=""]VAT number [vat_number_invo]<br>
[/c]
</div>

<hr style="width: 100%; border: 1px solid black;">

<div class="invoice_number" style="padding: 24px 0 8px 0">
Invoice n. [document_progressive_number] released on [today]
</div>


[r4 array="vat_perc_arr_invo"]
<table class="invoice_items" border="1" cellpadding="5" style="border: 1px black solid; width: 740px; margin-left: auto; margin-right: 5px; margin-top: 8px; border-collapse: collapse; background-color: #e6e6e6;">
[r]
[c show_rate_invo="1"]<tr><td style="border-color: black;">Stay from [starting_date] to [ending_date][people_phrase_invo]</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [rate_no_vat_invo_p]</td></tr>
[/c][c show_discount_invo="1"]<tr><td style="border-color: black;">Discount</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [discount_no_vat_invo_p]</td></tr>
[/c]
[r3][c show_extra_cost_invo="1"]<tr><td style="border-color: black;">Extra: "[extra_cost_name]"</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [extra_cost_no_vat_invo_p]</td></tr>
[/c][c show_cost_as_taxes_invo="1"]<tr><td style="border-color: black;">Tax: "[extra_cost_name]"</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [extra_cost_taxes_p]</td></tr>
[/c][/r3][/r]
[c vat_perc_arr_invo(vat_num_invo)="-1"]<!--[/c]
[c show_subtotal_invo="1"]<tr><td style="border-color: black;">Sub total at [vat_perc_arr_invo(vat_num_invo)]%</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [part_tot_no_vat_invo_p]</td></tr>
<tr><td style="border-color: black;">Taxes at [vat_perc_arr_invo(vat_num_invo)]%</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [part_tot_vat_invo_p]</td></tr>
[/c]
[c vat_perc_arr_invo(vat_num_invo)="-1"]-->[/c]
</table>
[/r4]

<table class="invoice_subtotal" border="1" cellpadding="5" style="border: 1px black solid; width: 740px; margin-left: auto; margin-right: 5px; margin-top: 8px; border-collapse: collapse; background-color: #e6e6e6;">
<tr><td style="border-color: black;">Sub total</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [tot_no_vat_invo_p]</td></tr>
<tr><td style="border-color: black;">Taxes[c vat_num_invo="1"] at [vat_perc_arr_invo(vat_num_invo)]%[/c] total</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [vat_invo_p]</td></tr>
[r][r3][c show_tax_cost_invo="1"]
<tr><td style="border-color: black;">[extra_cost_name]</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [extra_cost_no_vat_invo_p]</td></tr>
[/c][/r3][/r]
</table>

<table class="invoice_total" border="1" cellpadding="5" style="border: 1px black solid; width: 740px; margin-left: auto; margin-right: 5px; margin-top: 8px; border-collapse: collapse; background-color: #cccccc;">
<tr><td style="border-color: black;">Invoice total</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [price_tot_invo_p]</td></tr>
</table>


<br>
<hr style="width: 100%; border: 1px solid black;">
<br>


</div>
</div></cmp></riga>
<riga><cmp>4</cmp><cmp>contrhtm</cmp><cmp><div class="invoice_frame" style="width: 780px; margin: 10px; border: solid 1px black; padding: 30px; font-size: 12px; page-break-after: always">

<div class="structure_data">
[logo_recei]
<div class="structure_name" style="font-size: large;">[structure_type] [structure_name]</div>
[structure_company_name]<br>
[structure_address] - [structure_city]<br>
[structure_postal_code] [structure_nation]<br>
VAT number [structure_vat_number] [struct_fisc_code_recei]<br>
[struct_telephone_recei]<br>
</div>

<div class="client_data" style="padding: 18px 0 8px 440px;">
Receipt for [first_name_recei] [surname_recei]<br>
[c street_recei!=""][street_recei][street_num_recei]<br>
[/c][c city_row_recei!=""][city_row_recei]<br>
[/c][c nation_row_recei!=""][nation_row_recei]<br>
[/c][c fiscal_code!=""]Fiscal code [fiscal_code]<br>
[/c][c vat_number!=""]VAT number [vat_number]<br>
[/c]
</div>

<hr style="width: 100%; border: 1px solid black;">

<div class="invoice_number" style="padding: 24px 0 8px 0">
Receipt[c document_progressive_number!=""] n. [document_progressive_number][/c] released on [today]
</div>


<table class="invoice_items" border="1" cellpadding="5" style="border: 1px black solid; width: 740px; margin-left: auto; margin-right: 5px; margin-top: 8px; border-collapse: collapse; background-color: #e6e6e6;">
<tr><td style="border-color: black;">[c starting_date!=""]Reservation from [starting_date] to [ending_date][/c][c people_num_tot!=""] for [people_num_tot] persons[/c][r5][c starting_date="" & last_payment="1"][payment_method][/c][/r5]</td>
<td style="width: 120px; text-align: right; border-color: black;">[r5][c show_method_recei="1" & last_payment="1"][currency_name] [payment_value_p][/c][/r5]</td></tr>
</table>

<table class="invoice_total" border="1" cellpadding="5" style="border: 1px black solid; width: 740px; margin-left: auto; margin-right: 5px; margin-top: 8px; border-collapse: collapse; background-color: #cccccc;">
<tr><td style="border-color: black;">Total paid</td>
<td style="width: 120px; text-align: right; border-color: black;">[currency_name] [r5][c last_payment="1"][payment_value_p][/c][/r5]</td></tr>
</table>


<br>
<hr style="width: 100%; border: 1px solid black;">
<br>


</div></cmp></riga>
<riga><cmp>9</cmp><cmp>contrhtm</cmp><cmp>[r][null_value][r3][null_value][/r3][/r][r4 array="extra_costs_clean"][null_value][/r4][r4 array="array_dates_clean"][r][r3][null_value][/r3][/r][/r4]
<h1>Cleaning Report for <em>[f_report_date_clean]</em></h1>
<table class="clrep">
<tr class="headrow"><td>Room</td><td>People</td><td>Situation</td><td><small><small>Departing<br>People</small></small></td><td><small><small>Arrival<br>Time</small></small></td>[ec_table_head_clean]</tr>
[r6]
<tr[row_class_clean]><td>[unit_clean]</td><td>[people_clean(unit_clean)]</td><td>[situation_clean(unit_clean)]</td><td>[dep_people_clean(unit_clean)]</td><td>[arrival_time_clean(unit_clean)]</td>[ec_unit_row_clean(unit_clean)]</tr>
[repeat_header_row_clean]
[/r6]
<tr class="headrow"><td>Total</td><td>[people_tot_clean]</td><td><small>([arr_people_tot_clean] arrivals)</small></td><td>[dep_people_tot_clean]</td><td></td>
[r4 array="total_ec_clean"]
<td>[total_ec_clean(ec_num_clean)]</td>
[/r4]</tr>
</table></cmp></riga>
<riga><cmp>12</cmp><cmp>contrhtm</cmp><cmp></cmp></riga>
<riga><cmp>13</cmp><cmp>contrhtm</cmp><cmp></cmp></riga>
<riga><cmp>14</cmp><cmp>contrhtm</cmp><cmp></cmp></riga>
<riga><cmp>6</cmp><cmp>contreml</cmp><cmp>#!mln!#en</cmp></riga>
<riga><cmp>7</cmp><cmp>contreml</cmp><cmp>#!mln!#en</cmp></riga>
<riga><cmp>8</cmp><cmp>contreml</cmp><cmp>#!mln!#en</cmp></riga>
<riga><cmp>5</cmp><cmp>cond9</cmp><cmp>ind#@?#@?set#%?82#%?=#%?txt#%?30#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>6</cmp><cmp>cond9</cmp><cmp>ind#@?#@?date#%?75#%?data_inizio_selezione#%?is#%?2#%?g</cmp></riga>
<riga><cmp>7</cmp><cmp>cond9</cmp><cmp>ind#@?or#$?data_inizio_selezione#%?=#%?txt#%?#$?var_tmp_clean#%?>#%?var#%?data_fine_selezione#@?set#%?-1#%?=#%?txt#%?This document must be viewed from the table with departures and current reservations (entry "reservations" in top menu --&gt; "depart. and current") or at least 2 days must be selected.#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>8</cmp><cmp>cond9</cmp><cmp>ind#@?#@?date#%?64#%?data_inizio_selezione#%?is#%?1#%?g</cmp></riga>
<riga><cmp>9</cmp><cmp>cond9</cmp><cmp>rpt#@?#@?set#%?67#%?=#%?var#%?unita_occupata#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>10</cmp><cmp>cond9</cmp><cmp>rpt#@?#@?set#%?65#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>15</cmp><cmp>cond9</cmp><cmp>rpt#@?#@?set#%?66#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>16</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?data_inizio#%?=#%?var#%?report_date_clean#@?set#%?65#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>17</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?data_fine#%?=#%?var#%?report_date_clean#@?set#%?66#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>18</cmp><cmp>cond9</cmp><cmp>run#@?#@?set#%?67#%?=#%?var#%?nome_unita#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>19</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?arrival_clean#%?=#%?txt#%?1#$?situation_clean(unit_clean)#%?!=#%?txt#%?#@?set#%?a2322#%?=#%?txt#%?DEP+ARR#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>20</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?departure_clean#%?=#%?txt#%?1#$?situation_clean(unit_clean)#%?!=#%?txt#%?#@?set#%?a2322#%?=#%?txt#%?DEP+ARR#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>31</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?data_inizio#%?<#%?var#%?report_date_clean#$?data_fine#%?>#%?var#%?report_date_clean#@?set#%?a2322#%?=#%?txt#%?STAY#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>33</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?arrival_clean#%?=#%?txt#%?1#$?situation_clean(unit_clean)#%?=#%?txt#%?#@?set#%?a2322#%?=#%?txt#%?ARRIVAL#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>34</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?departure_clean#%?=#%?txt#%?1#$?situation_clean(unit_clean)#%?=#%?txt#%?#@?set#%?a2322#%?=#%?txt#%?DEPARTURE#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>35</cmp><cmp>cond9</cmp><cmp>ind#@?#@?date#%?69#%?report_date_clean#%?da#%?0#%?g</cmp></riga>
<riga><cmp>36</cmp><cmp>cond9</cmp><cmp>rpt#@?#@?set#%?75#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>37</cmp><cmp>cond9</cmp><cmp>rpt#@?or#$?data_inizio#%?=#%?var#%?report_date_clean#$?data_inizio#%?<#%?var#%?report_date_clean#@?set#%?75#%?=#%?var#%?num_persone#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>38</cmp><cmp>cond9</cmp><cmp>rpt#@?or#$?data_fine#%?=#%?var#%?report_date_clean#$?data_fine#%?<#%?var#%?report_date_clean#@?set#%?75#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>39</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?var_tmp_clean#%?!=#%?txt#%?#@?set#%?a2323#%?=#%?var#%?var_tmp_clean#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>40</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?var_tmp_clean#%?!=#%?txt#%?#$?n_letti_agg#%?!=#%?txt#%?#@?set#%?a2323#%?.=#%?txt#%?+#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>41</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?var_tmp_clean#%?!=#%?txt#%?#$?n_letti_agg#%?!=#%?txt#%?#@?set#%?a2323#%?.=#%?var#%?n_letti_agg#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>42</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?var_tmp_clean#%?!=#%?txt#%?#@?set#%?a2325#%?=#%?var#%?num_persone_tot#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>43</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?data_fine#%?=#%?var#%?report_date_clean#@?set#%?a2324#%?=#%?var#%?num_persone#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>44</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?data_fine#%?=#%?var#%?report_date_clean#$?n_letti_agg#%?!=#%?txt#%?#@?set#%?a2324#%?.=#%?txt#%?+#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>45</cmp><cmp>cond9</cmp><cmp>rpt#@?and#$?data_fine#%?=#%?var#%?report_date_clean#$?n_letti_agg#%?!=#%?txt#%?#@?set#%?a2324#%?.=#%?var#%?n_letti_agg#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>46</cmp><cmp>cond9</cmp><cmp>run#@?#@?oper#%?70#%?people_tot_clean#%?+#%?var#%?people_num_clean(unit_clean)#%?</cmp></riga>
<riga><cmp>47</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?data_fine#%?=#%?var#%?report_date_clean#@?set#%?a2326#%?=#%?var#%?num_persone_tot#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>48</cmp><cmp>cond9</cmp><cmp>run#@?#@?oper#%?71#%?dep_people_tot_clean#%?+#%?var#%?dep_people_num_clean(unit_clean)#%?</cmp></riga>
<riga><cmp>49</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?data_inizio#%?=#%?var#%?report_date_clean#@?set#%?a2327#%?=#%?var#%?num_persone_tot#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>50</cmp><cmp>cond9</cmp><cmp>run#@?#@?oper#%?72#%?arr_people_tot_clean#%?+#%?var#%?arr_people_num_clean(unit_clean)#%?</cmp></riga>
<riga><cmp>51</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?data_inizio#%?=#%?var#%?report_date_clean#@?set#%?a2328#%?=#%?var#%?orario_entrata_stimato#%?var#%?report_date_clean#%?txt#%?#%?</cmp></riga>
<riga><cmp>52</cmp><cmp>cond9</cmp><cmp>ind#@?#@?set#%?73#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>53</cmp><cmp>cond9</cmp><cmp>rca#@?#$?repetition_number_clean#%?!=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>54</cmp><cmp>cond9</cmp><cmp>rca#@?#@?set#%?75#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>55</cmp><cmp>cond9</cmp><cmp>rca#@?#$?giorni_costo_agg#%?{A}#%?txt#%?,#@?set#%?75#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>56</cmp><cmp>cond9</cmp><cmp>rca#@?#$?var_tmp_clean#%?=#%?txt#%?0#@?break#%?cont</cmp></riga>
<riga><cmp>57</cmp><cmp>cond9</cmp><cmp>rca#@?#$?ec_pos_clean(nome_costo_agg)#%?=#%?txt#%?#@?oper#%?73#%?ec_num_clean#%?+#%?txt#%?1#%?</cmp></riga>
<riga><cmp>58</cmp><cmp>cond9</cmp><cmp>rca#@?#$?ec_pos_clean(nome_costo_agg)#%?=#%?txt#%?#@?set#%?a2329#%?=#%?var#%?nome_costo_agg#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>59</cmp><cmp>cond9</cmp><cmp>ind#@?#@?set#%?74#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>60</cmp><cmp>cond9</cmp><cmp>inr#@?#@?oper#%?74#%?repetition_number_clean#%?+#%?txt#%?1#%?</cmp></riga>
<riga><cmp>61</cmp><cmp>cond9</cmp><cmp>rca#@?#$?ec_pos_clean(nome_costo_agg)#%?=#%?txt#%?#@?set#%?a2330#%?=#%?var#%?ec_num_clean#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>62</cmp><cmp>cond9</cmp><cmp>rca#@?#@?cont</cmp></riga>
<riga><cmp>63</cmp><cmp>cond9</cmp><cmp>run#@?#@?set#%?75#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>64</cmp><cmp>cond9</cmp><cmp>run#@?#$?row_class_clean#%?!=#%?txt#%?#@?set#%?75#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>65</cmp><cmp>cond9</cmp><cmp>run#@?#$?var_tmp_clean#%?!=#%?txt#%?1#@?set#%?68#%?=#%?txt#%? class="bgclr"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>66</cmp><cmp>cond9</cmp><cmp>run#@?#$?var_tmp_clean#%?=#%?txt#%?1#@?set#%?68#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>67</cmp><cmp>cond9</cmp><cmp>rar2329#@?#@?set#%?76#%?.=#%?txt#%?<td><small><small>#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>68</cmp><cmp>cond9</cmp><cmp>rar2329#@?#@?set#%?76#%?.=#%?var#%?extra_costs_clean(ec_num_clean)#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>69</cmp><cmp>cond9</cmp><cmp>rar2329#@?#@?set#%?76#%?.=#%?txt#%?</small></small></td>#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>71</cmp><cmp>cond9</cmp><cmp>rar2329#@?#@?set#%?77#%?.=#%?txt#%?<td><!-- cost#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>72</cmp><cmp>cond9</cmp><cmp>rar2329#@?#@?set#%?77#%?.=#%?var#%?ec_num_clean#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>73</cmp><cmp>cond9</cmp><cmp>rar2329#@?#@?set#%?77#%?.=#%?txt#%? --></td>#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>74</cmp><cmp>cond9</cmp><cmp>rca#@?#$?repetition_number_clean#%?<#%?txt#%?2#@?break#%?cont</cmp></riga>
<riga><cmp>75</cmp><cmp>cond9</cmp><cmp>rca#@?#$?array_dates_clean(day_clean)#%?!=#%?var#%?report_date_clean#@?break#%?cont</cmp></riga>
<riga><cmp>76</cmp><cmp>cond9</cmp><cmp>rca#@?#$?ec_pos_clean(nome_costo_agg)#%?=#%?txt#%?#@?break#%?cont</cmp></riga>
<riga><cmp>77</cmp><cmp>cond9</cmp><cmp>rca#@?#@?set#%?67#%?=#%?var#%?unita_occupata#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>78</cmp><cmp>cond9</cmp><cmp>rca#@?#$?ec_unit_row_clean(unit_clean)#%?=#%?txt#%?#@?set#%?a2332#%?=#%?var#%?ec_table_row_clean#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>79</cmp><cmp>cond9</cmp><cmp>ind#@?#@?array#%?a2333#%?dat#%?</cmp></riga>
<riga><cmp>80</cmp><cmp>cond9</cmp><cmp>rca#@?#@?set#%?75#%?=#%?txt#%?<!-- cost#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>82</cmp><cmp>cond9</cmp><cmp>rca#@?#@?set#%?75#%?.=#%?var#%?ec_pos_clean(nome_costo_agg)#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>83</cmp><cmp>cond9</cmp><cmp>rca#@?#@?set#%?75#%?.=#%?txt#%? -->#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>84</cmp><cmp>cond9</cmp><cmp>rca#@?#@?set#%?a2332#%?=#%?var#%?ec_unit_row_clean(unit_clean)#%?var#%?var_tmp_clean#%?var#%?moltiplica_max_costo_agg#%?</cmp></riga>
<riga><cmp>85</cmp><cmp>cond9</cmp><cmp>rca#@?#@?set#%?73#%?=#%?var#%?ec_pos_clean(nome_costo_agg)#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>86</cmp><cmp>cond9</cmp><cmp>rca#@?#@?oper#%?a2334#%?total_ec_clean(ec_num_clean)#%?+#%?var#%?moltiplica_max_costo_agg#%?</cmp></riga>
<riga><cmp>87</cmp><cmp>cond9</cmp><cmp>rca#@?#@?cont</cmp></riga>
<riga><cmp>88</cmp><cmp>cond9</cmp><cmp>run#@?#$?ec_unit_row_clean(unit_clean)#%?=#%?txt#%?#@?set#%?a2332#%?=#%?var#%?ec_table_row_clean#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>89</cmp><cmp>cond9</cmp><cmp>rpt#@?#$?data_inizio#%?=#%?var#%?report_date_clean#@?trunc#%?a2328#%?-3#%?#%?ini</cmp></riga>
<riga><cmp>90</cmp><cmp>cond9</cmp><cmp>rar2329#@?#@?set#%?a2334#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>91</cmp><cmp>cond9</cmp><cmp>ind#@?#@?set#%?79#%?=#%?txt#%?<tr class="headrow"><td>Room</td><td>People</td><td>Situation</td><td><small><small>Departing<br>People</small></small></td><td><small><small>Arrival<br>Time</small></small></td>#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>92</cmp><cmp>cond9</cmp><cmp>run#@?#@?oper#%?80#%?unit_repetition_number_clean#%?+#%?txt#%?1#%?</cmp></riga>
<riga><cmp>93</cmp><cmp>cond9</cmp><cmp>run#@?#@?set#%?81#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>94</cmp><cmp>cond9</cmp><cmp>run#@?#$?unit_repetition_number_clean#%?=#%?var#%?number_repeat_head_row_clean#@?set#%?81#%?=#%?var#%?header_row_table_clean#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>95</cmp><cmp>cond9</cmp><cmp>run#@?#$?unit_repetition_number_clean#%?=#%?var#%?number_repeat_head_row_clean#@?set#%?81#%?.=#%?var#%?ec_table_head_clean#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>96</cmp><cmp>cond9</cmp><cmp>run#@?#$?unit_repetition_number_clean#%?=#%?var#%?number_repeat_head_row_clean#@?set#%?81#%?.=#%?txt#%?</tr>#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>97</cmp><cmp>cond9</cmp><cmp>run#@?#$?unit_repetition_number_clean#%?=#%?var#%?number_repeat_head_row_clean#@?set#%?80#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>1</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?84#%?=#%?var#%?cognome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>2</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?85#%?=#%?var#%?cognome#%?txt#%? #%?txt#%?#%?</cmp></riga>
<riga><cmp>3</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?trunc#%?85#%?6#%?#%?ini</cmp></riga>
<riga><cmp>4</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?85#%?=#%?var#%?surn_no_sp_wle#%?txt#%?#%?txt#%?#%?url</cmp></riga>
<riga><cmp>5</cmp><cmp>cond8</cmp><cmp>rpt#@?#$?ultima_prenotazione_per_cliente#%?=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>6</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?var#%?url_base_pagine_web#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>7</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?txt#%?confirm_reservation_tpl.php?cn=#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>8</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?var#%?surn_no_sp_wle#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>9</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?txt#%?&cp=#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>10</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?var#%?codice_prenotazione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>11</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?txt#%?&fe=1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>12</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?var#%?avanzamento_riga#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>13</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2340#%?.=#%?var#%?avanzamento_riga#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>14</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?set#%?a2341#%?=#%?var#%?arr_links_wle(numero_cliente)#%?txt#%?confirm_reservation_tpl.php?cn=#%?txt#%?mdl_confirma_reserva.php?cn=#%?</cmp></riga>
<riga><cmp>15</cmp><cmp>cond8</cmp><cmp>rpt#@?#@?cont</cmp></riga>
<riga><cmp>16</cmp><cmp>cond8</cmp><cmp>rpt#@?#$?ultima_prenotazione_per_cliente#%?!=#%?txt#%?1#@?set#%?-2#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>1</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?20#%?=#%?var#%?cognome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>2</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?21#%?=#%?var#%?cognome#%?txt#%? #%?txt#%?#%?</cmp></riga>
<riga><cmp>3</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?trunc#%?21#%?6#%?#%?ini</cmp></riga>
<riga><cmp>4</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?21#%?=#%?var#%?surn_no_sp_cre#%?txt#%?#%?txt#%?#%?url</cmp></riga>
<riga><cmp>5</cmp><cmp>cond7</cmp><cmp>rpt#@?#$?ultima_prenotazione_per_cliente#%?=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>6</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2336#%?.=#%?var#%?url_base_pagine_web#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>7</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2336#%?.=#%?txt#%?confirm_reservation_tpl.php?cn=#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>8</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2336#%?.=#%?var#%?surn_no_sp_cre#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>9</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2336#%?.=#%?txt#%?&cp=#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>10</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2336#%?.=#%?var#%?codice_prenotazione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>11</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2336#%?.=#%?var#%?avanzamento_riga#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>12</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2336#%?.=#%?var#%?avanzamento_riga#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>13</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2337#%?=#%?var#%?arr_links_cre(numero_cliente)#%?txt#%?confirm_reservation_tpl.php?cn=#%?txt#%?mdl_confirma_reserva.php?cn=#%?</cmp></riga>
<riga><cmp>14</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2338#%?.=#%?txt#%?Surname: #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>15</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2338#%?.=#%?var#%?cognome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>16</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2338#%?.=#%?var#%?avanzamento_riga#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>17</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2338#%?.=#%?txt#%?Reservation code: #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>18</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2338#%?.=#%?var#%?codice_prenotazione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>19</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2338#%?.=#%?var#%?avanzamento_riga#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>20</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2338#%?.=#%?var#%?avanzamento_riga#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>21</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2339#%?=#%?var#%?arr_access_data_cre(numero_cliente)#%?txt#%?Surname:#%?txt#%?Apellido:#%?</cmp></riga>
<riga><cmp>22</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?set#%?a2339#%?=#%?var#%?arr_access_data_es_cre(numero_cliente)#%?txt#%?Reservation code:#%?txt#%?Código reserva:#%?</cmp></riga>
<riga><cmp>23</cmp><cmp>cond7</cmp><cmp>rpt#@?#@?cont</cmp></riga>
<riga><cmp>24</cmp><cmp>cond7</cmp><cmp>rpt#@?#$?ultima_prenotazione_per_cliente#%?!=#%?txt#%?1#@?set#%?-2#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>1</cmp><cmp>cond6</cmp><cmp>rpt#@?#@?set#%?19#%?=#%?var#%?cognome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>87</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?codice_fiscale_struttura#%?!=#%?txt#%?#@?set#%?14#%?=#%?txt#%?- Fiscal Code  #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>88</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?codice_fiscale_struttura#%?!=#%?txt#%?#@?set#%?14#%?.=#%?var#%?codice_fiscale_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>89</cmp><cmp>cond4</cmp><cmp>rpt#@?#@?set#%?17#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>92</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?telefono_struttura#%?!=#%?txt#%?#@?set#%?17#%?=#%?txt#%?Tel. #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>94</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?telefono_struttura#%?!=#%?txt#%?#@?set#%?17#%?.=#%?var#%?telefono_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>97</cmp><cmp>cond4</cmp><cmp>rpt#@?and#$?telefono_struttura#%?!=#%?txt#%?#$?sito_web_struttura#%?!=#%?txt#%?#@?set#%?17#%?.=#%?txt#%? - #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>98</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?sito_web_struttura#%?!=#%?txt#%?#@?set#%?17#%?.=#%?var#%?sito_web_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>99</cmp><cmp>cond4</cmp><cmp>rpt#@?#@?set#%?15#%?=#%?var#%?nome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>100</cmp><cmp>cond4</cmp><cmp>rpt#@?#@?set#%?16#%?=#%?var#%?cognome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>101</cmp><cmp>cond4</cmp><cmp>rpt#@?#@?set#%?18#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>102</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?numcivico#%?!=#%?txt#%?#@?set#%?18#%?=#%?txt#%?, #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>103</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?numcivico#%?!=#%?txt#%?#@?set#%?18#%?.=#%?var#%?numcivico#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>104</cmp><cmp>cond4</cmp><cmp>rpt#@?#@?set#%?12#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>105</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?citta#%?!=#%?txt#%?#@?set#%?12#%?.=#%?var#%?citta#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>107</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?regione#%?!=#%?txt#%?#@?set#%?12#%?.=#%?txt#%? (#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>108</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?regione#%?!=#%?txt#%?#@?set#%?12#%?.=#%?var#%?regione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>109</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?regione#%?!=#%?txt#%?#@?set#%?12#%?.=#%?txt#%?)#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>110</cmp><cmp>cond4</cmp><cmp>rpt#@?#@?set#%?13#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>111</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?cap#%?!=#%?txt#%?#@?set#%?13#%?.=#%?var#%?cap#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>112</cmp><cmp>cond4</cmp><cmp>rpt#@?and#$?cap#%?!=#%?txt#%?#$?nazione#%?!=#%?txt#%?#@?set#%?13#%?.=#%?txt#%? #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>113</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?nazione#%?!=#%?txt#%?#@?set#%?13#%?.=#%?var#%?nazione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>114</cmp><cmp>cond4</cmp><cmp>rpt#@?#@?set#%?57#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>115</cmp><cmp>cond4</cmp><cmp>rpa#@?and#$?data_inizio#%?=#%?txt#%?#$?ultimo_pagamento#%?=#%?txt#%?1#$?metodo_pagamento#%?!=#%?txt#%?#@?set#%?57#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>116</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?logo_struttura#%?!=#%?txt#%?#@?set#%?83#%?=#%?txt#%?<img src="#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>117</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?logo_struttura#%?!=#%?txt#%?#@?set#%?83#%?.=#%?var#%?logo_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>118</cmp><cmp>cond4</cmp><cmp>rpt#@?#$?logo_struttura#%?!=#%?txt#%?#@?set#%?83#%?.=#%?txt#%?" alt="Logo" style="float: right;">#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>25</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?45#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>26</cmp><cmp>cond2</cmp><cmp>rpt#@?and#$?vat_perc_arr_invo(vat_num_invo)#%?=#%?var#%?percentuale_tasse_tariffa#$?repetition_num_invo#%?>#%?txt#%?1#@?set#%?45#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>27</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?46#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>30</cmp><cmp>cond2</cmp><cmp>rpt#@?and#$?show_rate_invo#%?=#%?txt#%?1#$?sconto#%?!=#%?txt#%?0#@?set#%?46#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>31</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?47#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>32</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?62#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>36</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?63#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>38</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?59#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>39</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?nome_costo_agg#%?=#%?var#%?tax_cost_name_invo#@?set#%?59#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>40</cmp><cmp>cond2</cmp><cmp>rpt#@?and#$?vat_perc_arr_invo(vat_num_invo)#%?=#%?var#%?percentuale_tasse_costo_agg#$?valore_costo_agg#%?!=#%?txt#%?0#$?repetition_num_invo#%?>#%?txt#%?1#$?show_tax_cost_invo#%?!=#%?txt#%?1#@?set#%?47#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>42</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?28#%?=#%?var#%?percentuale_tasse_tariffa#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>43</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?tmp_var_invo#%?=#%?txt#%?#@?set#%?28#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>44</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?121#%?=#%?var#%?tmp_var_invo#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>46</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?exist_perc_vat_invo(tmp_var_invo)#%?=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>48</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?44#%?vat_num_invo#%?+#%?txt#%?1#%?</cmp></riga>
<riga><cmp>52</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?53#%?=#%?var#%?vat_num_invo#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>60</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?a1#%?=#%?var#%?tmp_var_invo#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>61</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?a2#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>65</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?cont</cmp></riga>
<riga><cmp>75</cmp><cmp>cond2</cmp><cmp>rpt#@?or#$?valore_costo_agg#%?=#%?txt#%?0#$?valore_costo_agg#%?=#%?txt#%?#$?show_tax_cost_invo#%?=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>76</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?28#%?=#%?var#%?percentuale_tasse_costo_agg#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>81</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?tmp_var_invo#%?=#%?txt#%?#@?set#%?28#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>83</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?exist_perc_vat_invo(tmp_var_invo)#%?=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>84</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?44#%?vat_num_invo#%?+#%?txt#%?1#%?</cmp></riga>
<riga><cmp>86</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?53#%?=#%?var#%?vat_num_invo#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>87</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?a1#%?=#%?var#%?tmp_var_invo#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>88</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?a2#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>89</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?cont</cmp></riga>
<riga><cmp>90</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?37#%?valore_costo_agg_senza_tasse#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>91</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_tax_cost_invo#%?=#%?txt#%?1#@?oper#%?37#%?valore_costo_agg#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>95</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?31#%?=#%?var#%?nome_costo_agg#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>100</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?32#%?tot_no_vat_invo#%?-#%?var#%?part_tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>105</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_extra_cost_invo#%?=#%?txt#%?1#@?oper#%?49#%?part_tot_no_vat_invo#%?+#%?var#%?valore_costo_agg_senza_tasse#%?</cmp></riga>
<riga><cmp>106</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_extra_cost_invo#%?=#%?txt#%?1#@?oper#%?50#%?part_tot_vat_invo#%?+#%?var#%?tasse_costo_agg#%?</cmp></riga>
<riga><cmp>108</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_extra_cost_invo#%?=#%?txt#%?1#@?oper#%?120#%?part_price_tot_invo#%?+#%?var#%?valore_costo_agg#%?</cmp></riga>
<riga><cmp>110</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_tax_cost_invo#%?=#%?txt#%?1#@?oper#%?61#%?tot_costs_tax_invo#%?+#%?var#%?valore_costo_agg#%?</cmp></riga>
<riga><cmp>111</cmp><cmp>cond2</cmp><cmp>rpt#@?or#$?round_on_totals_invo#%?!=#%?txt#%?YES#$?show_extra_cost_invo#%?!=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>112</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?28#%?tmp_var_invo#%?+#%?txt#%?100#%?</cmp></riga>
<riga><cmp>113</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?49#%?part_price_tot_invo#%?*#%?txt#%?100#%?</cmp></riga>
<riga><cmp>114</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?49#%?part_tot_no_vat_invo#%?/#%?var#%?tmp_var_invo#%?0.01</cmp></riga>
<riga><cmp>115</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?50#%?part_price_tot_invo#%?-#%?var#%?part_tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>116</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?cont</cmp></riga>
<riga><cmp>117</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?32#%?tot_no_vat_invo#%?+#%?var#%?part_tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>118</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?36#%?tot_no_vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>119</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?51#%?part_tot_no_vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>120</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?52#%?part_tot_vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>121</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?60#%?price_tot_invo#%?-#%?var#%?tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>122</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?60#%?vat_invo#%?-#%?var#%?tot_costs_tax_invo#%?</cmp></riga>
<riga><cmp>124</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?35#%?vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>125</cmp><cmp>cond2</cmp><cmp>rpt#@?or#$?show_extra_cost_invo#%?!=#%?txt#%?1#$?percentuale_tasse_costo_agg#%?!=#%?txt#%?-1#@?break#%?cont</cmp></riga>
<riga><cmp>128</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?62#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>129</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?47#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>130</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?cont</cmp></riga>
<riga><cmp>131</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?max_vat_num_invo#%?>#%?txt#%?1#@?set#%?63#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>132</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?last_reserv_invo#%?=#%?var#%?numero_prenotazione#@?break#%?</cmp></riga>
<riga><cmp>133</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?30#%?=#%?var#%?numero_prenotazione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>134</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?32#%?tot_no_vat_invo#%?-#%?var#%?part_tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>135</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_rate_invo#%?=#%?txt#%?1#@?oper#%?49#%?part_tot_no_vat_invo#%?+#%?var#%?costo_tariffa_senza_tasse#%?</cmp></riga>
<riga><cmp>136</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_rate_invo#%?=#%?txt#%?1#@?oper#%?50#%?part_tot_vat_invo#%?+#%?var#%?tasse_tariffa#%?</cmp></riga>
<riga><cmp>137</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_rate_invo#%?=#%?txt#%?1#@?oper#%?120#%?part_price_tot_invo#%?+#%?var#%?costo_tariffa#%?</cmp></riga>
<riga><cmp>138</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_discount_invo#%?=#%?txt#%?1#@?oper#%?49#%?part_tot_no_vat_invo#%?-#%?var#%?sconto_senza_tasse#%?</cmp></riga>
<riga><cmp>139</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_discount_invo#%?=#%?txt#%?1#@?oper#%?50#%?part_tot_vat_invo#%?-#%?var#%?tasse_sconto#%?</cmp></riga>
<riga><cmp>140</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?show_discount_invo#%?=#%?txt#%?1#@?oper#%?120#%?part_price_tot_invo#%?-#%?var#%?sconto#%?</cmp></riga>
<riga><cmp>141</cmp><cmp>cond2</cmp><cmp>rpt#@?or#$?round_on_totals_invo#%?!=#%?txt#%?YES#$?show_rate_invo#%?!=#%?txt#%?1#@?break#%?cont</cmp></riga>
<riga><cmp>142</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?28#%?rate_taxes_perc_invo#%?+#%?txt#%?100#%?</cmp></riga>
<riga><cmp>143</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?49#%?part_price_tot_invo#%?*#%?txt#%?100#%?</cmp></riga>
<riga><cmp>144</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?49#%?part_tot_no_vat_invo#%?/#%?var#%?tmp_var_invo#%?0.01</cmp></riga>
<riga><cmp>145</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?50#%?part_price_tot_invo#%?-#%?var#%?part_tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>146</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?cont</cmp></riga>
<riga><cmp>147</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?32#%?tot_no_vat_invo#%?+#%?var#%?part_tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>148</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?39#%?costo_tariffa_senza_tasse#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>149</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?38#%?sconto_senza_tasse#%?*#%?txt#%?-1#%?</cmp></riga>
<riga><cmp>150</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?36#%?tot_no_vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>151</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?51#%?part_tot_no_vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>152</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?52#%?part_tot_vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>153</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?60#%?price_tot_invo#%?-#%?var#%?tot_no_vat_invo#%?</cmp></riga>
<riga><cmp>154</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?60#%?vat_invo#%?-#%?var#%?tot_costs_tax_invo#%?</cmp></riga>
<riga><cmp>155</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?35#%?vat_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>156</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?merge_discount_with_rate#%?=#%?txt#%?YES#@?oper#%?39#%?costo_tariffa_senza_tasse#%?-#%?var#%?sconto_senza_tasse#%?</cmp></riga>
<riga><cmp>157</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?merge_discount_with_rate#%?=#%?txt#%?YES#@?set#%?46#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>158</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?54#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>159</cmp><cmp>cond2</cmp><cmp>rpt#@?and#$?num_persone_tot#%?!=#%?txt#%?#$?num_persone_tot#%?!=#%?txt#%?0#@?set#%?54#%?=#%?txt#%? for x persons#%?txt#%?x#%?var#%?num_persone_tot#%?</cmp></riga>
<riga><cmp>160</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?repetition_num_invo#%?>#%?txt#%?1#@?break#%?</cmp></riga>
<riga><cmp>161</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?33#%?price_tot_invo#%?+#%?var#%?costo_tot#%?</cmp></riga>
<riga><cmp>162</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?oper#%?34#%?price_tot_invo#%?+#%?txt#%?0#%?</cmp></riga>
<riga><cmp>163</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?codice_fiscale_struttura#%?!=#%?txt#%?#@?set#%?24#%?=#%?txt#%?- Fiscal Code #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>164</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?codice_fiscale_struttura#%?!=#%?txt#%?#@?set#%?24#%?.=#%?var#%?codice_fiscale_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>165</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?telefono_struttura#%?!=#%?txt#%?#@?set#%?27#%?=#%?txt#%?Tel. #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>166</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?telefono_struttura#%?!=#%?txt#%?#@?set#%?27#%?.=#%?var#%?telefono_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>167</cmp><cmp>cond2</cmp><cmp>inr#@?#@?set#%?30#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>168</cmp><cmp>cond2</cmp><cmp>inr#@?#@?oper#%?48#%?repetition_num_invo#%?+#%?txt#%?1#%?</cmp></riga>
<riga><cmp>169</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?55#%?=#%?txt#%?YES#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>170</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?119#%?=#%?txt#%?YES#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>171</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?58#%?=#%?txt#%?name of extra cost considered as tax#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>172</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?33#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>173</cmp><cmp>cond2</cmp><cmp>rpt#@?and#$?telefono_struttura#%?!=#%?txt#%?#$?sito_web_struttura#%?!=#%?txt#%?#@?set#%?27#%?.=#%?txt#%? - #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>174</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?sito_web_struttura#%?!=#%?txt#%?#@?set#%?27#%?.=#%?var#%?sito_web_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>175</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?32#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>176</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?25#%?=#%?var#%?nome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>177</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?26#%?=#%?var#%?cognome#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>178</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?40#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>179</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?numcivico#%?!=#%?txt#%?#@?set#%?40#%?=#%?txt#%?, #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>180</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?numcivico#%?!=#%?txt#%?#@?set#%?40#%?.=#%?var#%?numcivico#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>181</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?22#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>182</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?citta#%?!=#%?txt#%?#@?set#%?22#%?.=#%?var#%?citta#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>183</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?regione#%?!=#%?txt#%?#@?set#%?22#%?.=#%?txt#%? (#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>184</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?regione#%?!=#%?txt#%?#@?set#%?22#%?.=#%?var#%?regione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>185</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?regione#%?!=#%?txt#%?#@?set#%?22#%?.=#%?txt#%?)#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>186</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?23#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>187</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?cap#%?!=#%?txt#%?#@?set#%?23#%?.=#%?var#%?cap#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>188</cmp><cmp>cond2</cmp><cmp>rpt#@?and#$?cap#%?!=#%?txt#%?#$?nazione#%?!=#%?txt#%?#@?set#%?23#%?.=#%?txt#%? #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>189</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?nazione#%?!=#%?txt#%?#@?set#%?23#%?.=#%?var#%?nazione#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>190</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?41#%?=#%?var#%?codice_fiscale#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>191</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?42#%?=#%?var#%?partita_iva#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>192</cmp><cmp>cond2</cmp><cmp>rpt#@?#@?set#%?43#%?=#%?var#%?via#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>193</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?44#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>194</cmp><cmp>cond2</cmp><cmp>ind#@?#@?set#%?48#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>195</cmp><cmp>cond2</cmp><cmp>inr#@?#@?set#%?49#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>196</cmp><cmp>cond2</cmp><cmp>inr#@?#@?set#%?50#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>197</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?logo_struttura#%?!=#%?txt#%?#@?set#%?56#%?=#%?txt#%?<img src="#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>198</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?logo_struttura#%?!=#%?txt#%?#@?set#%?56#%?.=#%?var#%?logo_struttura#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>199</cmp><cmp>cond2</cmp><cmp>rpt#@?#$?logo_struttura#%?!=#%?txt#%?#@?set#%?56#%?.=#%?txt#%?" alt="Logo" style="float: right;">#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>200</cmp><cmp>cond2</cmp><cmp>inr#@?#@?set#%?61#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>201</cmp><cmp>cond2</cmp><cmp>inr#@?#@?set#%?120#%?=#%?txt#%?0#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>1</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?106#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>2</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?cognome#%?{}#%?txt#%?&quot;#$?cognome#%?{}#%?txt#%?,#@?set#%?106#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>3</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?106#%?.=#%?var#%?cognome#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>4</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?cognome#%?{}#%?txt#%?&quot;#$?cognome#%?{}#%?txt#%?,#@?set#%?106#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>5</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?107#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>6</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?nome#%?{}#%?txt#%?&quot;#$?nome#%?{}#%?txt#%?,#@?set#%?107#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>7</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?107#%?.=#%?var#%?nome#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>8</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?nome#%?{}#%?txt#%?&quot;#$?nome#%?{}#%?txt#%?,#@?set#%?107#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>9</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?108#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>10</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?unita_occupata#%?{}#%?txt#%?&quot;#$?unita_occupata#%?{}#%?txt#%?,#@?set#%?108#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>11</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?108#%?.=#%?var#%?unita_occupata#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>12</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?unita_occupata#%?{}#%?txt#%?&quot;#$?unita_occupata#%?{}#%?txt#%?,#@?set#%?108#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>13</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?109#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>14</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?nome_tariffa#%?{}#%?txt#%?&quot;#$?nome_tariffa#%?{}#%?txt#%?,#@?set#%?109#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>15</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?109#%?.=#%?var#%?nome_tariffa#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>16</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?nome_tariffa#%?{}#%?txt#%?&quot;#$?nome_tariffa#%?{}#%?txt#%?,#@?set#%?109#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>17</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?110#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>18</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?email#%?{}#%?txt#%?&quot;#$?email#%?{}#%?txt#%?,#@?set#%?110#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>19</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?110#%?.=#%?var#%?email#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>20</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?email#%?{}#%?txt#%?&quot;#$?email#%?{}#%?txt#%?,#@?set#%?110#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>21</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?111#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>22</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?telefono#%?{}#%?txt#%?&quot;#$?telefono#%?{}#%?txt#%?,#@?set#%?111#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>23</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?111#%?.=#%?var#%?telefono#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>24</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?telefono#%?{}#%?txt#%?&quot;#$?telefono#%?{}#%?txt#%?,#@?set#%?111#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>25</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?112#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>26</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?costo_tariffa#%?{}#%?txt#%?&quot;#$?costo_tariffa#%?{}#%?txt#%?,#@?set#%?112#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>27</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?112#%?.=#%?var#%?costo_tariffa#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>28</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?costo_tariffa#%?{}#%?txt#%?&quot;#$?costo_tariffa#%?{}#%?txt#%?,#@?set#%?112#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>29</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?113#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>30</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?costo_tot#%?{}#%?txt#%?&quot;#$?costo_tot#%?{}#%?txt#%?,#@?set#%?113#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>31</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?113#%?.=#%?var#%?costo_tot#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>32</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?costo_tot#%?{}#%?txt#%?&quot;#$?costo_tot#%?{}#%?txt#%?,#@?set#%?113#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>33</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?114#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>34</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?pagato#%?{}#%?txt#%?&quot;#$?pagato#%?{}#%?txt#%?,#@?set#%?114#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>35</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?114#%?.=#%?var#%?pagato#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>36</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?pagato#%?{}#%?txt#%?&quot;#$?pagato#%?{}#%?txt#%?,#@?set#%?114#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>37</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?115#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>38</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?num_persone_tot#%?{}#%?txt#%?&quot;#$?num_persone_tot#%?{}#%?txt#%?,#@?set#%?115#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>39</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?115#%?.=#%?var#%?num_persone_tot#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>40</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?num_persone_tot#%?{}#%?txt#%?&quot;#$?num_persone_tot#%?{}#%?txt#%?,#@?set#%?115#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>48</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?116#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>49</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?commento#%?{}#%?txt#%?&quot;#$?commento#%?{}#%?txt#%?,#@?set#%?116#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>50</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?116#%?.=#%?var#%?commento#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>51</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?set#%?116#%?=#%?var#%?comment_rcsv#%?var#%?avanzamento_riga#%?txt#%?#%?</cmp></riga>
<riga><cmp>56</cmp><cmp>cond11</cmp><cmp>rpt#@?or#$?commento#%?{}#%?txt#%?&quot;#$?commento#%?{}#%?txt#%?,#@?set#%?116#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>79</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?date#%?117#%?data_inizio#%?is#%?0#%?g</cmp></riga>
<riga><cmp>80</cmp><cmp>cond11</cmp><cmp>rpt#@?#@?date#%?118#%?data_fine#%?is#%?0#%?g</cmp></riga>
<riga><cmp>1</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?86#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>2</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?cognome#%?{}#%?txt#%?&quot;#$?cognome#%?{}#%?txt#%?,#@?set#%?86#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>3</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?86#%?.=#%?var#%?cognome#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>4</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?cognome#%?{}#%?txt#%?&quot;#$?cognome#%?{}#%?txt#%?,#@?set#%?86#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>5</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?87#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>6</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?nome#%?{}#%?txt#%?&quot;#$?nome#%?{}#%?txt#%?,#@?set#%?87#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>7</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?87#%?.=#%?var#%?nome#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>8</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?nome#%?{}#%?txt#%?&quot;#$?nome#%?{}#%?txt#%?,#@?set#%?87#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>9</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?88#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>10</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?soprannome#%?{}#%?txt#%?&quot;#$?soprannome#%?{}#%?txt#%?,#@?set#%?88#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>11</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?88#%?.=#%?var#%?soprannome#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>12</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?soprannome#%?{}#%?txt#%?&quot;#$?soprannome#%?{}#%?txt#%?,#@?set#%?88#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>13</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?89#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>14</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?titolo#%?{}#%?txt#%?&quot;#$?titolo#%?{}#%?txt#%?,#@?set#%?89#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>15</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?89#%?.=#%?var#%?titolo#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>16</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?titolo#%?{}#%?txt#%?&quot;#$?titolo#%?{}#%?txt#%?,#@?set#%?89#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>17</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?90#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>18</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?email#%?{}#%?txt#%?&quot;#$?email#%?{}#%?txt#%?,#@?set#%?90#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>19</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?90#%?.=#%?var#%?email#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>20</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?email#%?{}#%?txt#%?&quot;#$?email#%?{}#%?txt#%?,#@?set#%?90#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>21</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?91#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>22</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?telefono#%?{}#%?txt#%?&quot;#$?telefono#%?{}#%?txt#%?,#@?set#%?91#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>23</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?91#%?.=#%?var#%?telefono#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>24</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?telefono#%?{}#%?txt#%?&quot;#$?telefono#%?{}#%?txt#%?,#@?set#%?91#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>25</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?92#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>26</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?fax#%?{}#%?txt#%?&quot;#$?fax#%?{}#%?txt#%?,#@?set#%?92#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>27</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?92#%?.=#%?var#%?fax#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>28</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?fax#%?{}#%?txt#%?&quot;#$?fax#%?{}#%?txt#%?,#@?set#%?92#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>29</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?93#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>30</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?nazione#%?{}#%?txt#%?&quot;#$?nazione#%?{}#%?txt#%?,#@?set#%?93#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>31</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?93#%?.=#%?var#%?nazione#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>32</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?nazione#%?{}#%?txt#%?&quot;#$?nazione#%?{}#%?txt#%?,#@?set#%?93#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>33</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?94#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>34</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?regione#%?{}#%?txt#%?&quot;#$?regione#%?{}#%?txt#%?,#@?set#%?94#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>35</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?94#%?.=#%?var#%?regione#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>36</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?regione#%?{}#%?txt#%?&quot;#$?regione#%?{}#%?txt#%?,#@?set#%?94#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>37</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?95#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>38</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?citta#%?{}#%?txt#%?&quot;#$?citta#%?{}#%?txt#%?,#@?set#%?95#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>39</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?95#%?.=#%?var#%?citta#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>40</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?citta#%?{}#%?txt#%?&quot;#$?citta#%?{}#%?txt#%?,#@?set#%?95#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>41</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?101#%?=#%?var#%?via#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>42</cmp><cmp>cond10</cmp><cmp>rpt#@?#$?numcivico#%?!=#%?txt#%?#@?set#%?101#%?.=#%?txt#%? #%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>43</cmp><cmp>cond10</cmp><cmp>rpt#@?#$?numcivico#%?!=#%?txt#%?#@?set#%?101#%?.=#%?var#%?numcivico#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>44</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?96#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>45</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?tmp_csv#%?{}#%?txt#%?&quot;#$?tmp_csv#%?{}#%?txt#%?,#@?set#%?96#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>46</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?96#%?.=#%?var#%?tmp_csv#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>47</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?tmp_csv#%?{}#%?txt#%?&quot;#$?tmp_csv#%?{}#%?txt#%?,#@?set#%?96#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>48</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?97#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>49</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?cap#%?{}#%?txt#%?&quot;#$?cap#%?{}#%?txt#%?,#@?set#%?97#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>50</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?97#%?.=#%?var#%?cap#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>51</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?cap#%?{}#%?txt#%?&quot;#$?cap#%?{}#%?txt#%?,#@?set#%?97#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>52</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?98#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>53</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?cittadinanza#%?{}#%?txt#%?&quot;#$?cittadinanza#%?{}#%?txt#%?,#@?set#%?98#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>54</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?98#%?.=#%?var#%?cittadinanza#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>55</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?cittadinanza#%?{}#%?txt#%?&quot;#$?cittadinanza#%?{}#%?txt#%?,#@?set#%?98#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>56</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?99#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>57</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?date#%?99#%?data_nascita#%?da#%?0#%?g</cmp></riga>
<riga><cmp>58</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?100#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>59</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?partita_iva#%?{}#%?txt#%?&quot;#$?partita_iva#%?{}#%?txt#%?,#@?set#%?100#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>60</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?100#%?.=#%?var#%?partita_iva#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>61</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?partita_iva#%?{}#%?txt#%?&quot;#$?partita_iva#%?{}#%?txt#%?,#@?set#%?100#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>62</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?102#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>63</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?email2#%?{}#%?txt#%?&quot;#$?email2#%?{}#%?txt#%?,#@?set#%?102#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>64</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?102#%?.=#%?var#%?email2#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>65</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?email2#%?{}#%?txt#%?&quot;#$?email2#%?{}#%?txt#%?,#@?set#%?102#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>66</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?103#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>67</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?email_certificata#%?{}#%?txt#%?&quot;#$?email_certificata#%?{}#%?txt#%?,#@?set#%?103#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>68</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?103#%?.=#%?var#%?email_certificata#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>69</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?email_certificata#%?{}#%?txt#%?&quot;#$?email_certificata#%?{}#%?txt#%?,#@?set#%?103#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>70</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?104#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>71</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?telefono2#%?{}#%?txt#%?&quot;#$?telefono2#%?{}#%?txt#%?,#@?set#%?104#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>72</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?104#%?.=#%?var#%?telefono2#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>73</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?telefono2#%?{}#%?txt#%?&quot;#$?telefono2#%?{}#%?txt#%?,#@?set#%?104#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>74</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?105#%?=#%?txt#%?#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>75</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?telefono3#%?{}#%?txt#%?&quot;#$?telefono3#%?{}#%?txt#%?,#@?set#%?105#%?=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>76</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?105#%?.=#%?var#%?telefono3#%?txt#%?&quot;#%?txt#%?""#%?</cmp></riga>
<riga><cmp>77</cmp><cmp>cond10</cmp><cmp>rpt#@?or#$?telefono3#%?{}#%?txt#%?&quot;#$?telefono3#%?{}#%?txt#%?,#@?set#%?105#%?.=#%?txt#%?"#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>78</cmp><cmp>cond10</cmp><cmp>rpt#@?#$?client_shown_csv(numero_cliente)#%?=#%?txt#%?1#@?set#%?-2#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>79</cmp><cmp>cond10</cmp><cmp>rpt#@?#@?set#%?a2335#%?=#%?txt#%?1#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>1</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?1#%?=#%?txt#%?s#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>3</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?2#%?=#%?txt#%?il#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>4</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?2#%?=#%?txt#%?la#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>5</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?3#%?=#%?txt#%?Il#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>6</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?3#%?=#%?txt#%?La#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>7</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?4#%?=#%?txt#%?al#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>8</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?4#%?=#%?txt#%?alla#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>9</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?5#%?=#%?txt#%?e#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>10</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?5#%?=#%?txt#%?a#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>11</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?6#%?=#%?txt#%?o#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>12</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?6#%?=#%?txt#%?a#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>23</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?7#%?=#%?txt#%?el#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>24</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?7#%?=#%?txt#%?la#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>25</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?8#%?=#%?txt#%?El#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>26</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?8#%?=#%?txt#%?La#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>27</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?9#%?=#%?txt#%?al#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>28</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?9#%?=#%?txt#%?a la#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>29</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?10#%?=#%?txt#%?a#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>30</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?!=#%?txt#%?f#@?set#%?11#%?=#%?txt#%?o#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>31</cmp><cmp>cond</cmp><cmp>rpt#@?#$?sesso#%?=#%?txt#%?f#@?set#%?11#%?=#%?txt#%?a#%?txt#%?#%?txt#%?#%?</cmp></riga>
<riga><cmp>2</cmp><cmp>compress</cmp><cmp>gz</cmp></riga>
<riga><cmp>3</cmp><cmp>compress</cmp><cmp>gz</cmp></riga>
<riga><cmp>6</cmp><cmp>allegato</cmp><cmp>0</cmp></riga>
<riga><cmp>7</cmp><cmp>allegato</cmp><cmp>0</cmp></riga>
<riga><cmp>8</cmp><cmp>allegato</cmp><cmp>0</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>cache</nometabella>
<colonnetabella>
<nomecolonna>numero</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_modifica</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>interconnessioni</nometabella>
<colonnetabella>
<nomecolonna>idlocale</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idremoto1</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idremoto2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipoid</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_ic</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>anno</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>2</cmp><cmp></cmp><cmp></cmp><cmp>id_utente_az</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>messaggi</nometabella>
<colonnetabella>
<nomecolonna>idmessaggi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_messaggio</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>stato</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idutenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idutenti_visto</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datavisione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>mittente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>testo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio1</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio4</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio5</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio6</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio7</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio8</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio9</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio10</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio11</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio12</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio13</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio14</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio15</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio16</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio17</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio18</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio19</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio20</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio21</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>dati_messaggio22</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>sistema</cmp><cmp></cmp><cmp>,1,</cmp><cmp>,</cmp><cmp>2025-11-16 11:00:10</cmp><cmp>1</cmp><cmp><div style="max-width: 600px; line-height: 1.1;"><h4>Welcome to HotelDruid!</h4><br>These are some simple steps you can follow to set up the basic functionality of HotelDruid:<br>
<ul style="line-height: 1.2;">
<li>Insert the information about the rooms from the
 <em><b><a href="./visualizza_tabelle.php?tipo_tabella=appartamenti&amp;<sessione>">rooms table</a></b></em>, 
 using the specific button below it. The rooms can be created, deleted and renamed. 
 It is recommended to insert at least the maximum capacity for each room.<br><br></li>
<li>Insert the number of rates, a name for each one of them and the corresponding prices from the 
 <em><b><a href="./creaprezzi.php?<sessione>">page to insert prices</a></b></em>.
 Consider that HotelDruid rates also act as room types (view next step).<br><br></li>
<li>Assign a list of rooms to each rate, inserting an assignment rule 2 for each one of them, from the 
 <em><b><a href="./crearegole.php?<sessione>#regola2">page to insert rules</a></b></em>.
 Each room can be assigned to more than one rate.<br><br></li>
<li>If this is a public web server you can enable the login and create new users from the
 <em><b><a href="./gestione_utenti.php?<sessione>">users management page</a></b></em>.<br><br></li>
<li>Go to the
 "<em><b><a href="./personalizza.php?<sessione>">configure and customize</a></b></em>"
 page to change currency name, enable registration of check-in, insert payment methods, and set up much more options.<br><br></li>
</ul></div></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>2025-11-16 11:00:10</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>prenota2025</nometabella>
<colonnetabella>
<nomecolonna>idprenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idclienti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idappartamenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>iddatainizio</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>iddatafine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>assegnazioneapp</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>app_assegnabili</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>num_persone</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cat_persone</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idprenota_compagna</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffesettimanali</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>incompatibilita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>sconto</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa_tot</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>caparra</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>commissioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tasseperc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>pagato</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valuta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>metodo_pagamento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>origine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>commento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>conferma</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>checkin</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>checkout</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>id_anni_prec</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_modifica</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>1</cmp><cmp>15</cmp><cmp>331</cmp><cmp>341</cmp><cmp>v</cmp><cmp></cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp>Rack#@&715#@&p</cmp><cmp>65,65,65,65,65,65,65,65,65,65,65;15,15,15,15,15,15,15,15,15,15,15;15,15,15,15,15,15,15,15,15,15,15</cmp><cmp></cmp><cmp></cmp><cmp>715</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>shss</cmp><cmp></cmp><cmp></cmp><cmp>N</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>2025-11-16 20:12:57</cmp><cmp>127.0.0.1</cmp><cmp></cmp><cmp>1</cmp></riga>
<riga><cmp>2</cmp><cmp>2</cmp><cmp>201</cmp><cmp>332</cmp><cmp>346</cmp><cmp>v</cmp><cmp></cmp><cmp>3</cmp><cmp></cmp><cmp></cmp><cmp>Rack#@&1425#@&p</cmp><cmp>95,95,95,95,95,95,95,95,95,95,95,95,95,95,95;45,45,45,45,45,45,45,45,45,45,45,45,45,45,45;15,15,15,15,15,15,15,15,15,15,15,15,15,15,15</cmp><cmp></cmp><cmp></cmp><cmp>1425</cmp><cmp>50</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>kwrv</cmp><cmp></cmp><cmp></cmp><cmp>S</cmp><cmp></cmp><cmp>2025-11-28 17:00:00</cmp><cmp></cmp><cmp>2025-11-23 20:49:15</cmp><cmp>127.0.0.1</cmp><cmp></cmp><cmp>1</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>prenotacanc2025</nometabella>
<colonnetabella>
<nomecolonna>idprenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idclienti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idappartamenti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>iddatainizio</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>iddatafine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>assegnazioneapp</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>app_assegnabili</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>num_persone</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cat_persone</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idprenota_compagna</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffesettimanali</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>incompatibilita</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>sconto</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa_tot</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>caparra</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>commissioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tasseperc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>pagato</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valuta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>metodo_pagamento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>codice</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>origine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>commento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>conferma</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>checkin</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>checkout</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>id_anni_prec</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_modifica</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>costiprenota2025</nometabella>
<colonnetabella>
<nomecolonna>idcostiprenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idprenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valore</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valore_perc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>arrotonda</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tasseperc</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>associasett</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>settimane</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>moltiplica</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>categoria</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>letto</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>cat_persone</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numlimite</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idntariffe</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>variazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>varmoltiplica</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>varnumsett</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>varperiodipermessi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>varbeniinv</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>varappincompatibili</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>vartariffeassociate</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>vartariffeincomp</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>3</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>rclientiprenota2025</nometabella>
<colonnetabella>
<nomecolonna>idprenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>idclienti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>num_ordine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>parentela</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>1</cmp><cmp>1</cmp><cmp></cmp><cmp>2025-11-16 20:12:57</cmp><cmp>127.0.0.1</cmp><cmp>1</cmp></riga>
<riga><cmp>2</cmp><cmp>2</cmp><cmp>1</cmp><cmp></cmp><cmp>2025-11-23 20:49:15</cmp><cmp>127.0.0.1</cmp><cmp>1</cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>periodi2025</nometabella>
<colonnetabella>
<nomecolonna>idperiodi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainizio</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datafine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa1</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa1p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa2p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa3p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa4</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa4p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa5</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa5p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa6</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa6p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa7</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa7p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa8</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa8p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa9</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa9p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa10</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa10p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa11</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa11p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa12</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa12p</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>2025-01-01</cmp><cmp>2025-01-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp>2025-01-02</cmp><cmp>2025-01-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp>2025-01-03</cmp><cmp>2025-01-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>4</cmp><cmp>2025-01-04</cmp><cmp>2025-01-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>5</cmp><cmp>2025-01-05</cmp><cmp>2025-01-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>6</cmp><cmp>2025-01-06</cmp><cmp>2025-01-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>7</cmp><cmp>2025-01-07</cmp><cmp>2025-01-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>8</cmp><cmp>2025-01-08</cmp><cmp>2025-01-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>9</cmp><cmp>2025-01-09</cmp><cmp>2025-01-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>10</cmp><cmp>2025-01-10</cmp><cmp>2025-01-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>11</cmp><cmp>2025-01-11</cmp><cmp>2025-01-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>12</cmp><cmp>2025-01-12</cmp><cmp>2025-01-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>13</cmp><cmp>2025-01-13</cmp><cmp>2025-01-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>14</cmp><cmp>2025-01-14</cmp><cmp>2025-01-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>15</cmp><cmp>2025-01-15</cmp><cmp>2025-01-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>16</cmp><cmp>2025-01-16</cmp><cmp>2025-01-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>17</cmp><cmp>2025-01-17</cmp><cmp>2025-01-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>18</cmp><cmp>2025-01-18</cmp><cmp>2025-01-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>19</cmp><cmp>2025-01-19</cmp><cmp>2025-01-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>20</cmp><cmp>2025-01-20</cmp><cmp>2025-01-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>21</cmp><cmp>2025-01-21</cmp><cmp>2025-01-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>22</cmp><cmp>2025-01-22</cmp><cmp>2025-01-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>23</cmp><cmp>2025-01-23</cmp><cmp>2025-01-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>24</cmp><cmp>2025-01-24</cmp><cmp>2025-01-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>25</cmp><cmp>2025-01-25</cmp><cmp>2025-01-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>26</cmp><cmp>2025-01-26</cmp><cmp>2025-01-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>27</cmp><cmp>2025-01-27</cmp><cmp>2025-01-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>28</cmp><cmp>2025-01-28</cmp><cmp>2025-01-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>29</cmp><cmp>2025-01-29</cmp><cmp>2025-01-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>30</cmp><cmp>2025-01-30</cmp><cmp>2025-01-31</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>31</cmp><cmp>2025-01-31</cmp><cmp>2025-02-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>32</cmp><cmp>2025-02-01</cmp><cmp>2025-02-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>33</cmp><cmp>2025-02-02</cmp><cmp>2025-02-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>34</cmp><cmp>2025-02-03</cmp><cmp>2025-02-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>35</cmp><cmp>2025-02-04</cmp><cmp>2025-02-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>36</cmp><cmp>2025-02-05</cmp><cmp>2025-02-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>37</cmp><cmp>2025-02-06</cmp><cmp>2025-02-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>38</cmp><cmp>2025-02-07</cmp><cmp>2025-02-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>39</cmp><cmp>2025-02-08</cmp><cmp>2025-02-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>40</cmp><cmp>2025-02-09</cmp><cmp>2025-02-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>41</cmp><cmp>2025-02-10</cmp><cmp>2025-02-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>42</cmp><cmp>2025-02-11</cmp><cmp>2025-02-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>43</cmp><cmp>2025-02-12</cmp><cmp>2025-02-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>44</cmp><cmp>2025-02-13</cmp><cmp>2025-02-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>45</cmp><cmp>2025-02-14</cmp><cmp>2025-02-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>46</cmp><cmp>2025-02-15</cmp><cmp>2025-02-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>47</cmp><cmp>2025-02-16</cmp><cmp>2025-02-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>48</cmp><cmp>2025-02-17</cmp><cmp>2025-02-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>49</cmp><cmp>2025-02-18</cmp><cmp>2025-02-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>50</cmp><cmp>2025-02-19</cmp><cmp>2025-02-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>51</cmp><cmp>2025-02-20</cmp><cmp>2025-02-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>52</cmp><cmp>2025-02-21</cmp><cmp>2025-02-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>53</cmp><cmp>2025-02-22</cmp><cmp>2025-02-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>54</cmp><cmp>2025-02-23</cmp><cmp>2025-02-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>55</cmp><cmp>2025-02-24</cmp><cmp>2025-02-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>56</cmp><cmp>2025-02-25</cmp><cmp>2025-02-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>57</cmp><cmp>2025-02-26</cmp><cmp>2025-02-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>58</cmp><cmp>2025-02-27</cmp><cmp>2025-02-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>59</cmp><cmp>2025-02-28</cmp><cmp>2025-03-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>60</cmp><cmp>2025-03-01</cmp><cmp>2025-03-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>61</cmp><cmp>2025-03-02</cmp><cmp>2025-03-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>62</cmp><cmp>2025-03-03</cmp><cmp>2025-03-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>63</cmp><cmp>2025-03-04</cmp><cmp>2025-03-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>64</cmp><cmp>2025-03-05</cmp><cmp>2025-03-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>65</cmp><cmp>2025-03-06</cmp><cmp>2025-03-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>66</cmp><cmp>2025-03-07</cmp><cmp>2025-03-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>67</cmp><cmp>2025-03-08</cmp><cmp>2025-03-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>68</cmp><cmp>2025-03-09</cmp><cmp>2025-03-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>69</cmp><cmp>2025-03-10</cmp><cmp>2025-03-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>70</cmp><cmp>2025-03-11</cmp><cmp>2025-03-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>71</cmp><cmp>2025-03-12</cmp><cmp>2025-03-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>72</cmp><cmp>2025-03-13</cmp><cmp>2025-03-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>73</cmp><cmp>2025-03-14</cmp><cmp>2025-03-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>74</cmp><cmp>2025-03-15</cmp><cmp>2025-03-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>75</cmp><cmp>2025-03-16</cmp><cmp>2025-03-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>76</cmp><cmp>2025-03-17</cmp><cmp>2025-03-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>77</cmp><cmp>2025-03-18</cmp><cmp>2025-03-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>78</cmp><cmp>2025-03-19</cmp><cmp>2025-03-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>79</cmp><cmp>2025-03-20</cmp><cmp>2025-03-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>80</cmp><cmp>2025-03-21</cmp><cmp>2025-03-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>81</cmp><cmp>2025-03-22</cmp><cmp>2025-03-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>82</cmp><cmp>2025-03-23</cmp><cmp>2025-03-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>83</cmp><cmp>2025-03-24</cmp><cmp>2025-03-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>84</cmp><cmp>2025-03-25</cmp><cmp>2025-03-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>85</cmp><cmp>2025-03-26</cmp><cmp>2025-03-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>86</cmp><cmp>2025-03-27</cmp><cmp>2025-03-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>87</cmp><cmp>2025-03-28</cmp><cmp>2025-03-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>88</cmp><cmp>2025-03-29</cmp><cmp>2025-03-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>89</cmp><cmp>2025-03-30</cmp><cmp>2025-03-31</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>90</cmp><cmp>2025-03-31</cmp><cmp>2025-04-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>91</cmp><cmp>2025-04-01</cmp><cmp>2025-04-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>92</cmp><cmp>2025-04-02</cmp><cmp>2025-04-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>93</cmp><cmp>2025-04-03</cmp><cmp>2025-04-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>94</cmp><cmp>2025-04-04</cmp><cmp>2025-04-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>95</cmp><cmp>2025-04-05</cmp><cmp>2025-04-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>96</cmp><cmp>2025-04-06</cmp><cmp>2025-04-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>97</cmp><cmp>2025-04-07</cmp><cmp>2025-04-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>98</cmp><cmp>2025-04-08</cmp><cmp>2025-04-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>99</cmp><cmp>2025-04-09</cmp><cmp>2025-04-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>100</cmp><cmp>2025-04-10</cmp><cmp>2025-04-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>101</cmp><cmp>2025-04-11</cmp><cmp>2025-04-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>102</cmp><cmp>2025-04-12</cmp><cmp>2025-04-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>103</cmp><cmp>2025-04-13</cmp><cmp>2025-04-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>104</cmp><cmp>2025-04-14</cmp><cmp>2025-04-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>105</cmp><cmp>2025-04-15</cmp><cmp>2025-04-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>106</cmp><cmp>2025-04-16</cmp><cmp>2025-04-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>107</cmp><cmp>2025-04-17</cmp><cmp>2025-04-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>108</cmp><cmp>2025-04-18</cmp><cmp>2025-04-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>109</cmp><cmp>2025-04-19</cmp><cmp>2025-04-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>110</cmp><cmp>2025-04-20</cmp><cmp>2025-04-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>111</cmp><cmp>2025-04-21</cmp><cmp>2025-04-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>112</cmp><cmp>2025-04-22</cmp><cmp>2025-04-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>113</cmp><cmp>2025-04-23</cmp><cmp>2025-04-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>114</cmp><cmp>2025-04-24</cmp><cmp>2025-04-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>115</cmp><cmp>2025-04-25</cmp><cmp>2025-04-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>116</cmp><cmp>2025-04-26</cmp><cmp>2025-04-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>117</cmp><cmp>2025-04-27</cmp><cmp>2025-04-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>118</cmp><cmp>2025-04-28</cmp><cmp>2025-04-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>119</cmp><cmp>2025-04-29</cmp><cmp>2025-04-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>120</cmp><cmp>2025-04-30</cmp><cmp>2025-05-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>121</cmp><cmp>2025-05-01</cmp><cmp>2025-05-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>122</cmp><cmp>2025-05-02</cmp><cmp>2025-05-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>123</cmp><cmp>2025-05-03</cmp><cmp>2025-05-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>124</cmp><cmp>2025-05-04</cmp><cmp>2025-05-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>125</cmp><cmp>2025-05-05</cmp><cmp>2025-05-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>126</cmp><cmp>2025-05-06</cmp><cmp>2025-05-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>127</cmp><cmp>2025-05-07</cmp><cmp>2025-05-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>128</cmp><cmp>2025-05-08</cmp><cmp>2025-05-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>129</cmp><cmp>2025-05-09</cmp><cmp>2025-05-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>130</cmp><cmp>2025-05-10</cmp><cmp>2025-05-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>131</cmp><cmp>2025-05-11</cmp><cmp>2025-05-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>132</cmp><cmp>2025-05-12</cmp><cmp>2025-05-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>133</cmp><cmp>2025-05-13</cmp><cmp>2025-05-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>134</cmp><cmp>2025-05-14</cmp><cmp>2025-05-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>135</cmp><cmp>2025-05-15</cmp><cmp>2025-05-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>136</cmp><cmp>2025-05-16</cmp><cmp>2025-05-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>137</cmp><cmp>2025-05-17</cmp><cmp>2025-05-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>138</cmp><cmp>2025-05-18</cmp><cmp>2025-05-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>139</cmp><cmp>2025-05-19</cmp><cmp>2025-05-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>140</cmp><cmp>2025-05-20</cmp><cmp>2025-05-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>141</cmp><cmp>2025-05-21</cmp><cmp>2025-05-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>142</cmp><cmp>2025-05-22</cmp><cmp>2025-05-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>143</cmp><cmp>2025-05-23</cmp><cmp>2025-05-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>144</cmp><cmp>2025-05-24</cmp><cmp>2025-05-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>145</cmp><cmp>2025-05-25</cmp><cmp>2025-05-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>146</cmp><cmp>2025-05-26</cmp><cmp>2025-05-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>147</cmp><cmp>2025-05-27</cmp><cmp>2025-05-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>148</cmp><cmp>2025-05-28</cmp><cmp>2025-05-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>149</cmp><cmp>2025-05-29</cmp><cmp>2025-05-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>150</cmp><cmp>2025-05-30</cmp><cmp>2025-05-31</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>151</cmp><cmp>2025-05-31</cmp><cmp>2025-06-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>152</cmp><cmp>2025-06-01</cmp><cmp>2025-06-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>153</cmp><cmp>2025-06-02</cmp><cmp>2025-06-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>154</cmp><cmp>2025-06-03</cmp><cmp>2025-06-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>155</cmp><cmp>2025-06-04</cmp><cmp>2025-06-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>156</cmp><cmp>2025-06-05</cmp><cmp>2025-06-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>157</cmp><cmp>2025-06-06</cmp><cmp>2025-06-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>158</cmp><cmp>2025-06-07</cmp><cmp>2025-06-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>159</cmp><cmp>2025-06-08</cmp><cmp>2025-06-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>160</cmp><cmp>2025-06-09</cmp><cmp>2025-06-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>161</cmp><cmp>2025-06-10</cmp><cmp>2025-06-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>162</cmp><cmp>2025-06-11</cmp><cmp>2025-06-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>163</cmp><cmp>2025-06-12</cmp><cmp>2025-06-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>164</cmp><cmp>2025-06-13</cmp><cmp>2025-06-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>165</cmp><cmp>2025-06-14</cmp><cmp>2025-06-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>166</cmp><cmp>2025-06-15</cmp><cmp>2025-06-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>167</cmp><cmp>2025-06-16</cmp><cmp>2025-06-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>168</cmp><cmp>2025-06-17</cmp><cmp>2025-06-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>169</cmp><cmp>2025-06-18</cmp><cmp>2025-06-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>170</cmp><cmp>2025-06-19</cmp><cmp>2025-06-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>171</cmp><cmp>2025-06-20</cmp><cmp>2025-06-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>172</cmp><cmp>2025-06-21</cmp><cmp>2025-06-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>173</cmp><cmp>2025-06-22</cmp><cmp>2025-06-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>174</cmp><cmp>2025-06-23</cmp><cmp>2025-06-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>175</cmp><cmp>2025-06-24</cmp><cmp>2025-06-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>176</cmp><cmp>2025-06-25</cmp><cmp>2025-06-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>177</cmp><cmp>2025-06-26</cmp><cmp>2025-06-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>178</cmp><cmp>2025-06-27</cmp><cmp>2025-06-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>179</cmp><cmp>2025-06-28</cmp><cmp>2025-06-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>180</cmp><cmp>2025-06-29</cmp><cmp>2025-06-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>181</cmp><cmp>2025-06-30</cmp><cmp>2025-07-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>182</cmp><cmp>2025-07-01</cmp><cmp>2025-07-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>183</cmp><cmp>2025-07-02</cmp><cmp>2025-07-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>184</cmp><cmp>2025-07-03</cmp><cmp>2025-07-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>185</cmp><cmp>2025-07-04</cmp><cmp>2025-07-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>186</cmp><cmp>2025-07-05</cmp><cmp>2025-07-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>187</cmp><cmp>2025-07-06</cmp><cmp>2025-07-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>188</cmp><cmp>2025-07-07</cmp><cmp>2025-07-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>189</cmp><cmp>2025-07-08</cmp><cmp>2025-07-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>190</cmp><cmp>2025-07-09</cmp><cmp>2025-07-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>191</cmp><cmp>2025-07-10</cmp><cmp>2025-07-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>192</cmp><cmp>2025-07-11</cmp><cmp>2025-07-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>193</cmp><cmp>2025-07-12</cmp><cmp>2025-07-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>194</cmp><cmp>2025-07-13</cmp><cmp>2025-07-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>195</cmp><cmp>2025-07-14</cmp><cmp>2025-07-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>196</cmp><cmp>2025-07-15</cmp><cmp>2025-07-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>197</cmp><cmp>2025-07-16</cmp><cmp>2025-07-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>198</cmp><cmp>2025-07-17</cmp><cmp>2025-07-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>199</cmp><cmp>2025-07-18</cmp><cmp>2025-07-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>200</cmp><cmp>2025-07-19</cmp><cmp>2025-07-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>201</cmp><cmp>2025-07-20</cmp><cmp>2025-07-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>202</cmp><cmp>2025-07-21</cmp><cmp>2025-07-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>203</cmp><cmp>2025-07-22</cmp><cmp>2025-07-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>204</cmp><cmp>2025-07-23</cmp><cmp>2025-07-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>205</cmp><cmp>2025-07-24</cmp><cmp>2025-07-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>206</cmp><cmp>2025-07-25</cmp><cmp>2025-07-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>207</cmp><cmp>2025-07-26</cmp><cmp>2025-07-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>208</cmp><cmp>2025-07-27</cmp><cmp>2025-07-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>209</cmp><cmp>2025-07-28</cmp><cmp>2025-07-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>210</cmp><cmp>2025-07-29</cmp><cmp>2025-07-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>211</cmp><cmp>2025-07-30</cmp><cmp>2025-07-31</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>212</cmp><cmp>2025-07-31</cmp><cmp>2025-08-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>213</cmp><cmp>2025-08-01</cmp><cmp>2025-08-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>214</cmp><cmp>2025-08-02</cmp><cmp>2025-08-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>215</cmp><cmp>2025-08-03</cmp><cmp>2025-08-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>216</cmp><cmp>2025-08-04</cmp><cmp>2025-08-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>217</cmp><cmp>2025-08-05</cmp><cmp>2025-08-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>218</cmp><cmp>2025-08-06</cmp><cmp>2025-08-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>219</cmp><cmp>2025-08-07</cmp><cmp>2025-08-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>220</cmp><cmp>2025-08-08</cmp><cmp>2025-08-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>221</cmp><cmp>2025-08-09</cmp><cmp>2025-08-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>222</cmp><cmp>2025-08-10</cmp><cmp>2025-08-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>223</cmp><cmp>2025-08-11</cmp><cmp>2025-08-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>224</cmp><cmp>2025-08-12</cmp><cmp>2025-08-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>225</cmp><cmp>2025-08-13</cmp><cmp>2025-08-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>226</cmp><cmp>2025-08-14</cmp><cmp>2025-08-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>227</cmp><cmp>2025-08-15</cmp><cmp>2025-08-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>228</cmp><cmp>2025-08-16</cmp><cmp>2025-08-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>229</cmp><cmp>2025-08-17</cmp><cmp>2025-08-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>230</cmp><cmp>2025-08-18</cmp><cmp>2025-08-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>231</cmp><cmp>2025-08-19</cmp><cmp>2025-08-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>232</cmp><cmp>2025-08-20</cmp><cmp>2025-08-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>233</cmp><cmp>2025-08-21</cmp><cmp>2025-08-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>234</cmp><cmp>2025-08-22</cmp><cmp>2025-08-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>235</cmp><cmp>2025-08-23</cmp><cmp>2025-08-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>236</cmp><cmp>2025-08-24</cmp><cmp>2025-08-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>237</cmp><cmp>2025-08-25</cmp><cmp>2025-08-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>238</cmp><cmp>2025-08-26</cmp><cmp>2025-08-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>239</cmp><cmp>2025-08-27</cmp><cmp>2025-08-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>240</cmp><cmp>2025-08-28</cmp><cmp>2025-08-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>241</cmp><cmp>2025-08-29</cmp><cmp>2025-08-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>242</cmp><cmp>2025-08-30</cmp><cmp>2025-08-31</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>243</cmp><cmp>2025-08-31</cmp><cmp>2025-09-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>244</cmp><cmp>2025-09-01</cmp><cmp>2025-09-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>245</cmp><cmp>2025-09-02</cmp><cmp>2025-09-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>246</cmp><cmp>2025-09-03</cmp><cmp>2025-09-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>247</cmp><cmp>2025-09-04</cmp><cmp>2025-09-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>248</cmp><cmp>2025-09-05</cmp><cmp>2025-09-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>249</cmp><cmp>2025-09-06</cmp><cmp>2025-09-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>250</cmp><cmp>2025-09-07</cmp><cmp>2025-09-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>251</cmp><cmp>2025-09-08</cmp><cmp>2025-09-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>252</cmp><cmp>2025-09-09</cmp><cmp>2025-09-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>253</cmp><cmp>2025-09-10</cmp><cmp>2025-09-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>254</cmp><cmp>2025-09-11</cmp><cmp>2025-09-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>255</cmp><cmp>2025-09-12</cmp><cmp>2025-09-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>256</cmp><cmp>2025-09-13</cmp><cmp>2025-09-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>257</cmp><cmp>2025-09-14</cmp><cmp>2025-09-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>258</cmp><cmp>2025-09-15</cmp><cmp>2025-09-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>259</cmp><cmp>2025-09-16</cmp><cmp>2025-09-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>260</cmp><cmp>2025-09-17</cmp><cmp>2025-09-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>261</cmp><cmp>2025-09-18</cmp><cmp>2025-09-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>262</cmp><cmp>2025-09-19</cmp><cmp>2025-09-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>263</cmp><cmp>2025-09-20</cmp><cmp>2025-09-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>264</cmp><cmp>2025-09-21</cmp><cmp>2025-09-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>265</cmp><cmp>2025-09-22</cmp><cmp>2025-09-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>266</cmp><cmp>2025-09-23</cmp><cmp>2025-09-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>267</cmp><cmp>2025-09-24</cmp><cmp>2025-09-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>268</cmp><cmp>2025-09-25</cmp><cmp>2025-09-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>269</cmp><cmp>2025-09-26</cmp><cmp>2025-09-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>270</cmp><cmp>2025-09-27</cmp><cmp>2025-09-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>271</cmp><cmp>2025-09-28</cmp><cmp>2025-09-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>272</cmp><cmp>2025-09-29</cmp><cmp>2025-09-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>273</cmp><cmp>2025-09-30</cmp><cmp>2025-10-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>274</cmp><cmp>2025-10-01</cmp><cmp>2025-10-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>275</cmp><cmp>2025-10-02</cmp><cmp>2025-10-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>276</cmp><cmp>2025-10-03</cmp><cmp>2025-10-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>277</cmp><cmp>2025-10-04</cmp><cmp>2025-10-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>278</cmp><cmp>2025-10-05</cmp><cmp>2025-10-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>279</cmp><cmp>2025-10-06</cmp><cmp>2025-10-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>280</cmp><cmp>2025-10-07</cmp><cmp>2025-10-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>281</cmp><cmp>2025-10-08</cmp><cmp>2025-10-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>282</cmp><cmp>2025-10-09</cmp><cmp>2025-10-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>283</cmp><cmp>2025-10-10</cmp><cmp>2025-10-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>284</cmp><cmp>2025-10-11</cmp><cmp>2025-10-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>285</cmp><cmp>2025-10-12</cmp><cmp>2025-10-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>286</cmp><cmp>2025-10-13</cmp><cmp>2025-10-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>287</cmp><cmp>2025-10-14</cmp><cmp>2025-10-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>288</cmp><cmp>2025-10-15</cmp><cmp>2025-10-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>289</cmp><cmp>2025-10-16</cmp><cmp>2025-10-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>290</cmp><cmp>2025-10-17</cmp><cmp>2025-10-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>291</cmp><cmp>2025-10-18</cmp><cmp>2025-10-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>292</cmp><cmp>2025-10-19</cmp><cmp>2025-10-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>293</cmp><cmp>2025-10-20</cmp><cmp>2025-10-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>294</cmp><cmp>2025-10-21</cmp><cmp>2025-10-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>295</cmp><cmp>2025-10-22</cmp><cmp>2025-10-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>296</cmp><cmp>2025-10-23</cmp><cmp>2025-10-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>297</cmp><cmp>2025-10-24</cmp><cmp>2025-10-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>298</cmp><cmp>2025-10-25</cmp><cmp>2025-10-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>299</cmp><cmp>2025-10-26</cmp><cmp>2025-10-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>300</cmp><cmp>2025-10-27</cmp><cmp>2025-10-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>301</cmp><cmp>2025-10-28</cmp><cmp>2025-10-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>302</cmp><cmp>2025-10-29</cmp><cmp>2025-10-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>303</cmp><cmp>2025-10-30</cmp><cmp>2025-10-31</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>304</cmp><cmp>2025-10-31</cmp><cmp>2025-11-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>305</cmp><cmp>2025-11-01</cmp><cmp>2025-11-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>306</cmp><cmp>2025-11-02</cmp><cmp>2025-11-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>307</cmp><cmp>2025-11-03</cmp><cmp>2025-11-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>308</cmp><cmp>2025-11-04</cmp><cmp>2025-11-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>309</cmp><cmp>2025-11-05</cmp><cmp>2025-11-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>310</cmp><cmp>2025-11-06</cmp><cmp>2025-11-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>311</cmp><cmp>2025-11-07</cmp><cmp>2025-11-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>312</cmp><cmp>2025-11-08</cmp><cmp>2025-11-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>313</cmp><cmp>2025-11-09</cmp><cmp>2025-11-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>314</cmp><cmp>2025-11-10</cmp><cmp>2025-11-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>315</cmp><cmp>2025-11-11</cmp><cmp>2025-11-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>316</cmp><cmp>2025-11-12</cmp><cmp>2025-11-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>317</cmp><cmp>2025-11-13</cmp><cmp>2025-11-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>318</cmp><cmp>2025-11-14</cmp><cmp>2025-11-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>319</cmp><cmp>2025-11-15</cmp><cmp>2025-11-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>320</cmp><cmp>2025-11-16</cmp><cmp>2025-11-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>321</cmp><cmp>2025-11-17</cmp><cmp>2025-11-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>322</cmp><cmp>2025-11-18</cmp><cmp>2025-11-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>323</cmp><cmp>2025-11-19</cmp><cmp>2025-11-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>324</cmp><cmp>2025-11-20</cmp><cmp>2025-11-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>325</cmp><cmp>2025-11-21</cmp><cmp>2025-11-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>326</cmp><cmp>2025-11-22</cmp><cmp>2025-11-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>327</cmp><cmp>2025-11-23</cmp><cmp>2025-11-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>328</cmp><cmp>2025-11-24</cmp><cmp>2025-11-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>329</cmp><cmp>2025-11-25</cmp><cmp>2025-11-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>330</cmp><cmp>2025-11-26</cmp><cmp>2025-11-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>331</cmp><cmp>2025-11-27</cmp><cmp>2025-11-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>332</cmp><cmp>2025-11-28</cmp><cmp>2025-11-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>333</cmp><cmp>2025-11-29</cmp><cmp>2025-11-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>334</cmp><cmp>2025-11-30</cmp><cmp>2025-12-01</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>335</cmp><cmp>2025-12-01</cmp><cmp>2025-12-02</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>336</cmp><cmp>2025-12-02</cmp><cmp>2025-12-03</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>337</cmp><cmp>2025-12-03</cmp><cmp>2025-12-04</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>338</cmp><cmp>2025-12-04</cmp><cmp>2025-12-05</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>339</cmp><cmp>2025-12-05</cmp><cmp>2025-12-06</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>340</cmp><cmp>2025-12-06</cmp><cmp>2025-12-07</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>341</cmp><cmp>2025-12-07</cmp><cmp>2025-12-08</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>342</cmp><cmp>2025-12-08</cmp><cmp>2025-12-09</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>343</cmp><cmp>2025-12-09</cmp><cmp>2025-12-10</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>344</cmp><cmp>2025-12-10</cmp><cmp>2025-12-11</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>345</cmp><cmp>2025-12-11</cmp><cmp>2025-12-12</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>346</cmp><cmp>2025-12-12</cmp><cmp>2025-12-13</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>347</cmp><cmp>2025-12-13</cmp><cmp>2025-12-14</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>348</cmp><cmp>2025-12-14</cmp><cmp>2025-12-15</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>349</cmp><cmp>2025-12-15</cmp><cmp>2025-12-16</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>350</cmp><cmp>2025-12-16</cmp><cmp>2025-12-17</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>351</cmp><cmp>2025-12-17</cmp><cmp>2025-12-18</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>352</cmp><cmp>2025-12-18</cmp><cmp>2025-12-19</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>353</cmp><cmp>2025-12-19</cmp><cmp>2025-12-20</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>354</cmp><cmp>2025-12-20</cmp><cmp>2025-12-21</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>355</cmp><cmp>2025-12-21</cmp><cmp>2025-12-22</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>356</cmp><cmp>2025-12-22</cmp><cmp>2025-12-23</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>357</cmp><cmp>2025-12-23</cmp><cmp>2025-12-24</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>358</cmp><cmp>2025-12-24</cmp><cmp>2025-12-25</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>359</cmp><cmp>2025-12-25</cmp><cmp>2025-12-26</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>360</cmp><cmp>2025-12-26</cmp><cmp>2025-12-27</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>361</cmp><cmp>2025-12-27</cmp><cmp>2025-12-28</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>362</cmp><cmp>2025-12-28</cmp><cmp>2025-12-29</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>363</cmp><cmp>2025-12-29</cmp><cmp>2025-12-30</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>364</cmp><cmp>2025-12-30</cmp><cmp>2025-12-31</cmp><cmp>50</cmp><cmp>15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>365</cmp><cmp>2025-12-31</cmp><cmp>2026-01-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>366</cmp><cmp>2026-01-01</cmp><cmp>2026-01-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>367</cmp><cmp>2026-01-02</cmp><cmp>2026-01-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>368</cmp><cmp>2026-01-03</cmp><cmp>2026-01-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>369</cmp><cmp>2026-01-04</cmp><cmp>2026-01-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>370</cmp><cmp>2026-01-05</cmp><cmp>2026-01-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>371</cmp><cmp>2026-01-06</cmp><cmp>2026-01-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>372</cmp><cmp>2026-01-07</cmp><cmp>2026-01-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>373</cmp><cmp>2026-01-08</cmp><cmp>2026-01-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>374</cmp><cmp>2026-01-09</cmp><cmp>2026-01-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>375</cmp><cmp>2026-01-10</cmp><cmp>2026-01-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>376</cmp><cmp>2026-01-11</cmp><cmp>2026-01-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>377</cmp><cmp>2026-01-12</cmp><cmp>2026-01-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>378</cmp><cmp>2026-01-13</cmp><cmp>2026-01-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>379</cmp><cmp>2026-01-14</cmp><cmp>2026-01-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>380</cmp><cmp>2026-01-15</cmp><cmp>2026-01-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>381</cmp><cmp>2026-01-16</cmp><cmp>2026-01-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>382</cmp><cmp>2026-01-17</cmp><cmp>2026-01-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>383</cmp><cmp>2026-01-18</cmp><cmp>2026-01-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>384</cmp><cmp>2026-01-19</cmp><cmp>2026-01-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>385</cmp><cmp>2026-01-20</cmp><cmp>2026-01-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>386</cmp><cmp>2026-01-21</cmp><cmp>2026-01-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>387</cmp><cmp>2026-01-22</cmp><cmp>2026-01-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>388</cmp><cmp>2026-01-23</cmp><cmp>2026-01-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>389</cmp><cmp>2026-01-24</cmp><cmp>2026-01-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>390</cmp><cmp>2026-01-25</cmp><cmp>2026-01-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>391</cmp><cmp>2026-01-26</cmp><cmp>2026-01-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>392</cmp><cmp>2026-01-27</cmp><cmp>2026-01-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>393</cmp><cmp>2026-01-28</cmp><cmp>2026-01-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>394</cmp><cmp>2026-01-29</cmp><cmp>2026-01-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>395</cmp><cmp>2026-01-30</cmp><cmp>2026-01-31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>396</cmp><cmp>2026-01-31</cmp><cmp>2026-02-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>397</cmp><cmp>2026-02-01</cmp><cmp>2026-02-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>398</cmp><cmp>2026-02-02</cmp><cmp>2026-02-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>399</cmp><cmp>2026-02-03</cmp><cmp>2026-02-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>400</cmp><cmp>2026-02-04</cmp><cmp>2026-02-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>401</cmp><cmp>2026-02-05</cmp><cmp>2026-02-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>402</cmp><cmp>2026-02-06</cmp><cmp>2026-02-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>403</cmp><cmp>2026-02-07</cmp><cmp>2026-02-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>404</cmp><cmp>2026-02-08</cmp><cmp>2026-02-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>405</cmp><cmp>2026-02-09</cmp><cmp>2026-02-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>406</cmp><cmp>2026-02-10</cmp><cmp>2026-02-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>407</cmp><cmp>2026-02-11</cmp><cmp>2026-02-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>408</cmp><cmp>2026-02-12</cmp><cmp>2026-02-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>409</cmp><cmp>2026-02-13</cmp><cmp>2026-02-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>410</cmp><cmp>2026-02-14</cmp><cmp>2026-02-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>411</cmp><cmp>2026-02-15</cmp><cmp>2026-02-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>412</cmp><cmp>2026-02-16</cmp><cmp>2026-02-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>413</cmp><cmp>2026-02-17</cmp><cmp>2026-02-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>414</cmp><cmp>2026-02-18</cmp><cmp>2026-02-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>415</cmp><cmp>2026-02-19</cmp><cmp>2026-02-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>416</cmp><cmp>2026-02-20</cmp><cmp>2026-02-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>417</cmp><cmp>2026-02-21</cmp><cmp>2026-02-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>418</cmp><cmp>2026-02-22</cmp><cmp>2026-02-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>419</cmp><cmp>2026-02-23</cmp><cmp>2026-02-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>420</cmp><cmp>2026-02-24</cmp><cmp>2026-02-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>421</cmp><cmp>2026-02-25</cmp><cmp>2026-02-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>422</cmp><cmp>2026-02-26</cmp><cmp>2026-02-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>423</cmp><cmp>2026-02-27</cmp><cmp>2026-02-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>424</cmp><cmp>2026-02-28</cmp><cmp>2026-03-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>425</cmp><cmp>2026-03-01</cmp><cmp>2026-03-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>426</cmp><cmp>2026-03-02</cmp><cmp>2026-03-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>427</cmp><cmp>2026-03-03</cmp><cmp>2026-03-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>428</cmp><cmp>2026-03-04</cmp><cmp>2026-03-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>429</cmp><cmp>2026-03-05</cmp><cmp>2026-03-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>430</cmp><cmp>2026-03-06</cmp><cmp>2026-03-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>431</cmp><cmp>2026-03-07</cmp><cmp>2026-03-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>432</cmp><cmp>2026-03-08</cmp><cmp>2026-03-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>433</cmp><cmp>2026-03-09</cmp><cmp>2026-03-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>434</cmp><cmp>2026-03-10</cmp><cmp>2026-03-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>435</cmp><cmp>2026-03-11</cmp><cmp>2026-03-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>436</cmp><cmp>2026-03-12</cmp><cmp>2026-03-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>437</cmp><cmp>2026-03-13</cmp><cmp>2026-03-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>438</cmp><cmp>2026-03-14</cmp><cmp>2026-03-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>439</cmp><cmp>2026-03-15</cmp><cmp>2026-03-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>440</cmp><cmp>2026-03-16</cmp><cmp>2026-03-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>441</cmp><cmp>2026-03-17</cmp><cmp>2026-03-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>442</cmp><cmp>2026-03-18</cmp><cmp>2026-03-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>443</cmp><cmp>2026-03-19</cmp><cmp>2026-03-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>444</cmp><cmp>2026-03-20</cmp><cmp>2026-03-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>445</cmp><cmp>2026-03-21</cmp><cmp>2026-03-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>446</cmp><cmp>2026-03-22</cmp><cmp>2026-03-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>447</cmp><cmp>2026-03-23</cmp><cmp>2026-03-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>448</cmp><cmp>2026-03-24</cmp><cmp>2026-03-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>449</cmp><cmp>2026-03-25</cmp><cmp>2026-03-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>450</cmp><cmp>2026-03-26</cmp><cmp>2026-03-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>451</cmp><cmp>2026-03-27</cmp><cmp>2026-03-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>452</cmp><cmp>2026-03-28</cmp><cmp>2026-03-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>453</cmp><cmp>2026-03-29</cmp><cmp>2026-03-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>454</cmp><cmp>2026-03-30</cmp><cmp>2026-03-31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>455</cmp><cmp>2026-03-31</cmp><cmp>2026-04-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>456</cmp><cmp>2026-04-01</cmp><cmp>2026-04-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>457</cmp><cmp>2026-04-02</cmp><cmp>2026-04-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>458</cmp><cmp>2026-04-03</cmp><cmp>2026-04-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>459</cmp><cmp>2026-04-04</cmp><cmp>2026-04-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>460</cmp><cmp>2026-04-05</cmp><cmp>2026-04-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>461</cmp><cmp>2026-04-06</cmp><cmp>2026-04-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>462</cmp><cmp>2026-04-07</cmp><cmp>2026-04-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>463</cmp><cmp>2026-04-08</cmp><cmp>2026-04-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>464</cmp><cmp>2026-04-09</cmp><cmp>2026-04-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>465</cmp><cmp>2026-04-10</cmp><cmp>2026-04-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>466</cmp><cmp>2026-04-11</cmp><cmp>2026-04-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>467</cmp><cmp>2026-04-12</cmp><cmp>2026-04-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>468</cmp><cmp>2026-04-13</cmp><cmp>2026-04-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>469</cmp><cmp>2026-04-14</cmp><cmp>2026-04-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>470</cmp><cmp>2026-04-15</cmp><cmp>2026-04-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>471</cmp><cmp>2026-04-16</cmp><cmp>2026-04-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>472</cmp><cmp>2026-04-17</cmp><cmp>2026-04-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>473</cmp><cmp>2026-04-18</cmp><cmp>2026-04-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>474</cmp><cmp>2026-04-19</cmp><cmp>2026-04-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>475</cmp><cmp>2026-04-20</cmp><cmp>2026-04-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>476</cmp><cmp>2026-04-21</cmp><cmp>2026-04-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>477</cmp><cmp>2026-04-22</cmp><cmp>2026-04-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>478</cmp><cmp>2026-04-23</cmp><cmp>2026-04-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>479</cmp><cmp>2026-04-24</cmp><cmp>2026-04-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>480</cmp><cmp>2026-04-25</cmp><cmp>2026-04-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>481</cmp><cmp>2026-04-26</cmp><cmp>2026-04-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>482</cmp><cmp>2026-04-27</cmp><cmp>2026-04-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>483</cmp><cmp>2026-04-28</cmp><cmp>2026-04-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>484</cmp><cmp>2026-04-29</cmp><cmp>2026-04-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>485</cmp><cmp>2026-04-30</cmp><cmp>2026-05-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>486</cmp><cmp>2026-05-01</cmp><cmp>2026-05-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>487</cmp><cmp>2026-05-02</cmp><cmp>2026-05-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>488</cmp><cmp>2026-05-03</cmp><cmp>2026-05-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>489</cmp><cmp>2026-05-04</cmp><cmp>2026-05-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>490</cmp><cmp>2026-05-05</cmp><cmp>2026-05-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>491</cmp><cmp>2026-05-06</cmp><cmp>2026-05-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>492</cmp><cmp>2026-05-07</cmp><cmp>2026-05-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>493</cmp><cmp>2026-05-08</cmp><cmp>2026-05-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>494</cmp><cmp>2026-05-09</cmp><cmp>2026-05-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>495</cmp><cmp>2026-05-10</cmp><cmp>2026-05-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>496</cmp><cmp>2026-05-11</cmp><cmp>2026-05-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>497</cmp><cmp>2026-05-12</cmp><cmp>2026-05-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>498</cmp><cmp>2026-05-13</cmp><cmp>2026-05-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>499</cmp><cmp>2026-05-14</cmp><cmp>2026-05-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>500</cmp><cmp>2026-05-15</cmp><cmp>2026-05-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>501</cmp><cmp>2026-05-16</cmp><cmp>2026-05-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>502</cmp><cmp>2026-05-17</cmp><cmp>2026-05-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>503</cmp><cmp>2026-05-18</cmp><cmp>2026-05-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>504</cmp><cmp>2026-05-19</cmp><cmp>2026-05-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>505</cmp><cmp>2026-05-20</cmp><cmp>2026-05-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>506</cmp><cmp>2026-05-21</cmp><cmp>2026-05-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>507</cmp><cmp>2026-05-22</cmp><cmp>2026-05-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>508</cmp><cmp>2026-05-23</cmp><cmp>2026-05-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>509</cmp><cmp>2026-05-24</cmp><cmp>2026-05-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>510</cmp><cmp>2026-05-25</cmp><cmp>2026-05-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>511</cmp><cmp>2026-05-26</cmp><cmp>2026-05-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>512</cmp><cmp>2026-05-27</cmp><cmp>2026-05-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>513</cmp><cmp>2026-05-28</cmp><cmp>2026-05-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>514</cmp><cmp>2026-05-29</cmp><cmp>2026-05-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>515</cmp><cmp>2026-05-30</cmp><cmp>2026-05-31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>516</cmp><cmp>2026-05-31</cmp><cmp>2026-06-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>517</cmp><cmp>2026-06-01</cmp><cmp>2026-06-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>518</cmp><cmp>2026-06-02</cmp><cmp>2026-06-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>519</cmp><cmp>2026-06-03</cmp><cmp>2026-06-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>520</cmp><cmp>2026-06-04</cmp><cmp>2026-06-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>521</cmp><cmp>2026-06-05</cmp><cmp>2026-06-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>522</cmp><cmp>2026-06-06</cmp><cmp>2026-06-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>523</cmp><cmp>2026-06-07</cmp><cmp>2026-06-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>524</cmp><cmp>2026-06-08</cmp><cmp>2026-06-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>525</cmp><cmp>2026-06-09</cmp><cmp>2026-06-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>526</cmp><cmp>2026-06-10</cmp><cmp>2026-06-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>527</cmp><cmp>2026-06-11</cmp><cmp>2026-06-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>528</cmp><cmp>2026-06-12</cmp><cmp>2026-06-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>529</cmp><cmp>2026-06-13</cmp><cmp>2026-06-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>530</cmp><cmp>2026-06-14</cmp><cmp>2026-06-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>531</cmp><cmp>2026-06-15</cmp><cmp>2026-06-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>532</cmp><cmp>2026-06-16</cmp><cmp>2026-06-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>533</cmp><cmp>2026-06-17</cmp><cmp>2026-06-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>534</cmp><cmp>2026-06-18</cmp><cmp>2026-06-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>535</cmp><cmp>2026-06-19</cmp><cmp>2026-06-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>536</cmp><cmp>2026-06-20</cmp><cmp>2026-06-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>537</cmp><cmp>2026-06-21</cmp><cmp>2026-06-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>538</cmp><cmp>2026-06-22</cmp><cmp>2026-06-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>539</cmp><cmp>2026-06-23</cmp><cmp>2026-06-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>540</cmp><cmp>2026-06-24</cmp><cmp>2026-06-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>541</cmp><cmp>2026-06-25</cmp><cmp>2026-06-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>542</cmp><cmp>2026-06-26</cmp><cmp>2026-06-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>543</cmp><cmp>2026-06-27</cmp><cmp>2026-06-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>544</cmp><cmp>2026-06-28</cmp><cmp>2026-06-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>545</cmp><cmp>2026-06-29</cmp><cmp>2026-06-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>546</cmp><cmp>2026-06-30</cmp><cmp>2026-07-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>547</cmp><cmp>2026-07-01</cmp><cmp>2026-07-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>548</cmp><cmp>2026-07-02</cmp><cmp>2026-07-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>549</cmp><cmp>2026-07-03</cmp><cmp>2026-07-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>550</cmp><cmp>2026-07-04</cmp><cmp>2026-07-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>551</cmp><cmp>2026-07-05</cmp><cmp>2026-07-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>552</cmp><cmp>2026-07-06</cmp><cmp>2026-07-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>553</cmp><cmp>2026-07-07</cmp><cmp>2026-07-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>554</cmp><cmp>2026-07-08</cmp><cmp>2026-07-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>555</cmp><cmp>2026-07-09</cmp><cmp>2026-07-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>556</cmp><cmp>2026-07-10</cmp><cmp>2026-07-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>557</cmp><cmp>2026-07-11</cmp><cmp>2026-07-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>558</cmp><cmp>2026-07-12</cmp><cmp>2026-07-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>559</cmp><cmp>2026-07-13</cmp><cmp>2026-07-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>560</cmp><cmp>2026-07-14</cmp><cmp>2026-07-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>561</cmp><cmp>2026-07-15</cmp><cmp>2026-07-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>562</cmp><cmp>2026-07-16</cmp><cmp>2026-07-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>563</cmp><cmp>2026-07-17</cmp><cmp>2026-07-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>564</cmp><cmp>2026-07-18</cmp><cmp>2026-07-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>565</cmp><cmp>2026-07-19</cmp><cmp>2026-07-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>566</cmp><cmp>2026-07-20</cmp><cmp>2026-07-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>567</cmp><cmp>2026-07-21</cmp><cmp>2026-07-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>568</cmp><cmp>2026-07-22</cmp><cmp>2026-07-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>569</cmp><cmp>2026-07-23</cmp><cmp>2026-07-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>570</cmp><cmp>2026-07-24</cmp><cmp>2026-07-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>571</cmp><cmp>2026-07-25</cmp><cmp>2026-07-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>572</cmp><cmp>2026-07-26</cmp><cmp>2026-07-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>573</cmp><cmp>2026-07-27</cmp><cmp>2026-07-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>574</cmp><cmp>2026-07-28</cmp><cmp>2026-07-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>575</cmp><cmp>2026-07-29</cmp><cmp>2026-07-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>576</cmp><cmp>2026-07-30</cmp><cmp>2026-07-31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>577</cmp><cmp>2026-07-31</cmp><cmp>2026-08-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>578</cmp><cmp>2026-08-01</cmp><cmp>2026-08-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>579</cmp><cmp>2026-08-02</cmp><cmp>2026-08-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>580</cmp><cmp>2026-08-03</cmp><cmp>2026-08-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>581</cmp><cmp>2026-08-04</cmp><cmp>2026-08-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>582</cmp><cmp>2026-08-05</cmp><cmp>2026-08-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>583</cmp><cmp>2026-08-06</cmp><cmp>2026-08-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>584</cmp><cmp>2026-08-07</cmp><cmp>2026-08-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>585</cmp><cmp>2026-08-08</cmp><cmp>2026-08-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>586</cmp><cmp>2026-08-09</cmp><cmp>2026-08-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>587</cmp><cmp>2026-08-10</cmp><cmp>2026-08-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>588</cmp><cmp>2026-08-11</cmp><cmp>2026-08-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>589</cmp><cmp>2026-08-12</cmp><cmp>2026-08-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>590</cmp><cmp>2026-08-13</cmp><cmp>2026-08-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>591</cmp><cmp>2026-08-14</cmp><cmp>2026-08-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>592</cmp><cmp>2026-08-15</cmp><cmp>2026-08-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>593</cmp><cmp>2026-08-16</cmp><cmp>2026-08-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>594</cmp><cmp>2026-08-17</cmp><cmp>2026-08-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>595</cmp><cmp>2026-08-18</cmp><cmp>2026-08-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>596</cmp><cmp>2026-08-19</cmp><cmp>2026-08-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>597</cmp><cmp>2026-08-20</cmp><cmp>2026-08-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>598</cmp><cmp>2026-08-21</cmp><cmp>2026-08-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>599</cmp><cmp>2026-08-22</cmp><cmp>2026-08-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>600</cmp><cmp>2026-08-23</cmp><cmp>2026-08-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>601</cmp><cmp>2026-08-24</cmp><cmp>2026-08-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>602</cmp><cmp>2026-08-25</cmp><cmp>2026-08-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>603</cmp><cmp>2026-08-26</cmp><cmp>2026-08-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>604</cmp><cmp>2026-08-27</cmp><cmp>2026-08-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>605</cmp><cmp>2026-08-28</cmp><cmp>2026-08-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>606</cmp><cmp>2026-08-29</cmp><cmp>2026-08-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>607</cmp><cmp>2026-08-30</cmp><cmp>2026-08-31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>608</cmp><cmp>2026-08-31</cmp><cmp>2026-09-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>609</cmp><cmp>2026-09-01</cmp><cmp>2026-09-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>610</cmp><cmp>2026-09-02</cmp><cmp>2026-09-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>611</cmp><cmp>2026-09-03</cmp><cmp>2026-09-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>612</cmp><cmp>2026-09-04</cmp><cmp>2026-09-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>613</cmp><cmp>2026-09-05</cmp><cmp>2026-09-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>614</cmp><cmp>2026-09-06</cmp><cmp>2026-09-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>615</cmp><cmp>2026-09-07</cmp><cmp>2026-09-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>616</cmp><cmp>2026-09-08</cmp><cmp>2026-09-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>617</cmp><cmp>2026-09-09</cmp><cmp>2026-09-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>618</cmp><cmp>2026-09-10</cmp><cmp>2026-09-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>619</cmp><cmp>2026-09-11</cmp><cmp>2026-09-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>620</cmp><cmp>2026-09-12</cmp><cmp>2026-09-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>621</cmp><cmp>2026-09-13</cmp><cmp>2026-09-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>622</cmp><cmp>2026-09-14</cmp><cmp>2026-09-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>623</cmp><cmp>2026-09-15</cmp><cmp>2026-09-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>624</cmp><cmp>2026-09-16</cmp><cmp>2026-09-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>625</cmp><cmp>2026-09-17</cmp><cmp>2026-09-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>626</cmp><cmp>2026-09-18</cmp><cmp>2026-09-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>627</cmp><cmp>2026-09-19</cmp><cmp>2026-09-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>628</cmp><cmp>2026-09-20</cmp><cmp>2026-09-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>629</cmp><cmp>2026-09-21</cmp><cmp>2026-09-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>630</cmp><cmp>2026-09-22</cmp><cmp>2026-09-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>631</cmp><cmp>2026-09-23</cmp><cmp>2026-09-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>632</cmp><cmp>2026-09-24</cmp><cmp>2026-09-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>633</cmp><cmp>2026-09-25</cmp><cmp>2026-09-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>634</cmp><cmp>2026-09-26</cmp><cmp>2026-09-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>635</cmp><cmp>2026-09-27</cmp><cmp>2026-09-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>636</cmp><cmp>2026-09-28</cmp><cmp>2026-09-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>637</cmp><cmp>2026-09-29</cmp><cmp>2026-09-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>638</cmp><cmp>2026-09-30</cmp><cmp>2026-10-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>639</cmp><cmp>2026-10-01</cmp><cmp>2026-10-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>640</cmp><cmp>2026-10-02</cmp><cmp>2026-10-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>641</cmp><cmp>2026-10-03</cmp><cmp>2026-10-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>642</cmp><cmp>2026-10-04</cmp><cmp>2026-10-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>643</cmp><cmp>2026-10-05</cmp><cmp>2026-10-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>644</cmp><cmp>2026-10-06</cmp><cmp>2026-10-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>645</cmp><cmp>2026-10-07</cmp><cmp>2026-10-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>646</cmp><cmp>2026-10-08</cmp><cmp>2026-10-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>647</cmp><cmp>2026-10-09</cmp><cmp>2026-10-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>648</cmp><cmp>2026-10-10</cmp><cmp>2026-10-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>649</cmp><cmp>2026-10-11</cmp><cmp>2026-10-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>650</cmp><cmp>2026-10-12</cmp><cmp>2026-10-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>651</cmp><cmp>2026-10-13</cmp><cmp>2026-10-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>652</cmp><cmp>2026-10-14</cmp><cmp>2026-10-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>653</cmp><cmp>2026-10-15</cmp><cmp>2026-10-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>654</cmp><cmp>2026-10-16</cmp><cmp>2026-10-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>655</cmp><cmp>2026-10-17</cmp><cmp>2026-10-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>656</cmp><cmp>2026-10-18</cmp><cmp>2026-10-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>657</cmp><cmp>2026-10-19</cmp><cmp>2026-10-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>658</cmp><cmp>2026-10-20</cmp><cmp>2026-10-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>659</cmp><cmp>2026-10-21</cmp><cmp>2026-10-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>660</cmp><cmp>2026-10-22</cmp><cmp>2026-10-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>661</cmp><cmp>2026-10-23</cmp><cmp>2026-10-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>662</cmp><cmp>2026-10-24</cmp><cmp>2026-10-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>663</cmp><cmp>2026-10-25</cmp><cmp>2026-10-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>664</cmp><cmp>2026-10-26</cmp><cmp>2026-10-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>665</cmp><cmp>2026-10-27</cmp><cmp>2026-10-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>666</cmp><cmp>2026-10-28</cmp><cmp>2026-10-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>667</cmp><cmp>2026-10-29</cmp><cmp>2026-10-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>668</cmp><cmp>2026-10-30</cmp><cmp>2026-10-31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>669</cmp><cmp>2026-10-31</cmp><cmp>2026-11-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>670</cmp><cmp>2026-11-01</cmp><cmp>2026-11-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>671</cmp><cmp>2026-11-02</cmp><cmp>2026-11-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>672</cmp><cmp>2026-11-03</cmp><cmp>2026-11-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>673</cmp><cmp>2026-11-04</cmp><cmp>2026-11-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>674</cmp><cmp>2026-11-05</cmp><cmp>2026-11-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>675</cmp><cmp>2026-11-06</cmp><cmp>2026-11-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>676</cmp><cmp>2026-11-07</cmp><cmp>2026-11-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>677</cmp><cmp>2026-11-08</cmp><cmp>2026-11-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>678</cmp><cmp>2026-11-09</cmp><cmp>2026-11-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>679</cmp><cmp>2026-11-10</cmp><cmp>2026-11-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>680</cmp><cmp>2026-11-11</cmp><cmp>2026-11-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>681</cmp><cmp>2026-11-12</cmp><cmp>2026-11-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>682</cmp><cmp>2026-11-13</cmp><cmp>2026-11-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>683</cmp><cmp>2026-11-14</cmp><cmp>2026-11-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>684</cmp><cmp>2026-11-15</cmp><cmp>2026-11-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>685</cmp><cmp>2026-11-16</cmp><cmp>2026-11-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>686</cmp><cmp>2026-11-17</cmp><cmp>2026-11-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>687</cmp><cmp>2026-11-18</cmp><cmp>2026-11-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>688</cmp><cmp>2026-11-19</cmp><cmp>2026-11-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>689</cmp><cmp>2026-11-20</cmp><cmp>2026-11-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>690</cmp><cmp>2026-11-21</cmp><cmp>2026-11-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>691</cmp><cmp>2026-11-22</cmp><cmp>2026-11-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>692</cmp><cmp>2026-11-23</cmp><cmp>2026-11-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>693</cmp><cmp>2026-11-24</cmp><cmp>2026-11-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>694</cmp><cmp>2026-11-25</cmp><cmp>2026-11-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>695</cmp><cmp>2026-11-26</cmp><cmp>2026-11-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>696</cmp><cmp>2026-11-27</cmp><cmp>2026-11-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>697</cmp><cmp>2026-11-28</cmp><cmp>2026-11-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>698</cmp><cmp>2026-11-29</cmp><cmp>2026-11-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>699</cmp><cmp>2026-11-30</cmp><cmp>2026-12-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>700</cmp><cmp>2026-12-01</cmp><cmp>2026-12-02</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>701</cmp><cmp>2026-12-02</cmp><cmp>2026-12-03</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>702</cmp><cmp>2026-12-03</cmp><cmp>2026-12-04</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>703</cmp><cmp>2026-12-04</cmp><cmp>2026-12-05</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>704</cmp><cmp>2026-12-05</cmp><cmp>2026-12-06</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>705</cmp><cmp>2026-12-06</cmp><cmp>2026-12-07</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>706</cmp><cmp>2026-12-07</cmp><cmp>2026-12-08</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>707</cmp><cmp>2026-12-08</cmp><cmp>2026-12-09</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>708</cmp><cmp>2026-12-09</cmp><cmp>2026-12-10</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>709</cmp><cmp>2026-12-10</cmp><cmp>2026-12-11</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>710</cmp><cmp>2026-12-11</cmp><cmp>2026-12-12</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>711</cmp><cmp>2026-12-12</cmp><cmp>2026-12-13</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>712</cmp><cmp>2026-12-13</cmp><cmp>2026-12-14</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>713</cmp><cmp>2026-12-14</cmp><cmp>2026-12-15</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>714</cmp><cmp>2026-12-15</cmp><cmp>2026-12-16</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>715</cmp><cmp>2026-12-16</cmp><cmp>2026-12-17</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>716</cmp><cmp>2026-12-17</cmp><cmp>2026-12-18</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>717</cmp><cmp>2026-12-18</cmp><cmp>2026-12-19</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>718</cmp><cmp>2026-12-19</cmp><cmp>2026-12-20</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>719</cmp><cmp>2026-12-20</cmp><cmp>2026-12-21</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>720</cmp><cmp>2026-12-21</cmp><cmp>2026-12-22</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>721</cmp><cmp>2026-12-22</cmp><cmp>2026-12-23</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>722</cmp><cmp>2026-12-23</cmp><cmp>2026-12-24</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>723</cmp><cmp>2026-12-24</cmp><cmp>2026-12-25</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>724</cmp><cmp>2026-12-25</cmp><cmp>2026-12-26</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>725</cmp><cmp>2026-12-26</cmp><cmp>2026-12-27</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>726</cmp><cmp>2026-12-27</cmp><cmp>2026-12-28</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>727</cmp><cmp>2026-12-28</cmp><cmp>2026-12-29</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>728</cmp><cmp>2026-12-29</cmp><cmp>2026-12-30</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>729</cmp><cmp>2026-12-30</cmp><cmp>2026-12-31</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>730</cmp><cmp>2026-12-31</cmp><cmp>2027-01-01</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>ntariffe2025</nometabella>
<colonnetabella>
<nomecolonna>idntariffe</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nomecostoagg</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valore_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valore_perc_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>arrotonda_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tasseperc_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>associasett_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numsett_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>moltiplica_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>periodipermessi_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>beniinv_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>appincompatibili_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>variazione_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>mostra_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>categoria_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>letto_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>numlimite_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>regoleassegna_ca</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa1</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa4</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa5</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa6</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa7</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa8</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa9</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa10</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa11</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa12</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>1</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>11</cmp><cmp></cmp><cmp></cmp><cmp>Rack</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>2</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>3</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>4</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>p</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>5</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
<riga><cmp>6</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>regole2025</nometabella>
<colonnetabella>
<nomecolonna>idregole</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>app_agenzia</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa_chiusa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa_per_app</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa_per_utente</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa_per_persone</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tariffa_commissioni</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>iddatainizio</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>iddatafine</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>motivazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>motivazione2</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>motivazione3</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
</righetabella>
</tabella>
<tabella>
<nometabella>soldi2025</nometabella>
<colonnetabella>
<nomecolonna>idsoldi</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>motivazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>id_pagamento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>metodo_pagamento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>note</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>saldo_prenota</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>saldo_cassa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>soldi_prima</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valuta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>saldo_valuta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_transazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_modifica</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>1</cmp><cmp>soldi_prenotazioni_cancellate</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp>0</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
<tabella>
<nometabella>costi2025</nometabella>
<colonnetabella>
<nomecolonna>idcosti</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_costo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>val_costo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>tipo_costo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>valuta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>costo_valuta</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>nome_cassa</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>persona_costo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>provenienza_costo</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>id_pagamento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>metodo_pagamento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_transazione</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>data_modifica</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>datainserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>hostinserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
<nomecolonna>utente_inserimento</nomecolonna>
<tipocolonna>unknown</tipocolonna>
</colonnetabella>
<righetabella>
<riga><cmp>0</cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp><cmp></cmp></riga>
</righetabella>
</tabella>
</database>
</backup>