// Functions for TOC

function exp(id) {
  var myElt=document.getElementById('p'+id);

  if (myElt) {
    // check current display state
    if (myElt.src.slice(myElt.src.lastIndexOf('/')+1) == 'minus.gif') {
      collapse(id);
    } else{
      expand(id);
    }
  }
}

function expand(id) {
  var myDoc= top.TOC.document;
  var myElt=myDoc.getElementById('s'+id);

  if (myElt) {
    with(myElt) {
      className='x';
      style.display=''; 
    }
    myDoc.getElementById('p'+id).src='minus.gif';
    myDoc.getElementById('b'+id).src='obook.gif';
  }
}

function collapse(id) {
  var myElt=document.getElementById('s'+id);

  if (myElt) {
    with(myElt) {
      className='x';
      style.display='none'; 
    }
    document.getElementById('p'+id).src='plus.gif';
    document.getElementById('b'+id).src='cbook.gif';
  }
}

function highlight(id) {
  var myElt=top.TOC.document.getElementById('a'+id);

  if (myElt) {
    myElt.hideFocus=true;
    //myElt.focus();
    myElt.setActive();
    top.TOC.scrollTo(myElt.offsetLeft-48, myElt.offsetTop-(top.TOC.document.body.clientHeight/3));
  }
}

function loadTOC() {
  // check current page displayed in TOC window.  If not toc.htm, load it.
  if (!isTOCLoaded()) {
    top.TOC.location.href='toc.htm';
  }
}

function isTOCLoaded() {
  // return true, if toc.htm is loaded in TOC window.
  if (top.TOC) {
    var myPath=top.TOC.location.pathname;
    var myFile=myPath.substr(myPath.length-7);

    if (myFile == 'toc.htm') {
      return true;
    } else {
      return false; 
    }
  } else {
    return false;
  }
}

