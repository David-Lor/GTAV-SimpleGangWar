"""EXTRACT CHANGELOG Script
This script extracts the changelog for the given version from the README.md file, under the `## Changelog` section.
The version is given as x.y.z (e.g. 0.1.1) as an arg (e.g. running "python extract_changelog.py 0.1.1").
The changelog is saved as markdown on a file.
"""

import sys
import re
from typing import List

from markdown2 import markdown
from bs4 import BeautifulSoup
from bs4.element import Tag

README_FILE = "README.md"
CHANGELOG_OUTPUT_FILE = "changelog_generated.md"


def _is_valid_version(version: str) -> bool:
    return bool(re.match(r"^(\d+\.)?(\d+\.)?(\*|\d+)$", version))


def _find_changelog_list(soup: BeautifulSoup, version: str) -> Tag:
    all_lists = soup.find_all("li")
    return next(li for li in all_lists if li.text.startswith(version))


def _parse_changelog(li: Tag) -> List[str]:
    all_changelog_lists = li.find("ul").find_all("li")
    return [li.text.strip() for li in all_changelog_lists]


def _format_changelog_output(changelog: List[str]) -> str:
    output = ""
    for change in changelog:
        output += f"\n- {change}"
    return output.strip()


def _read(filename: str) -> str:
    with open(filename, "r") as f:
        return f.read()


def _save(filename: str, content: str):
    with open(filename, "w") as f:
        f.write(content)


def extract_changelog(version: str):
    assert _is_valid_version(version)

    readme_content = _read(README_FILE)
    readme_html = markdown(readme_content)

    soup = BeautifulSoup(readme_html, "html.parser")
    changelog_list = _find_changelog_list(soup=soup, version=version)
    changelog_changes = _parse_changelog(changelog_list)
    changelog_output = _format_changelog_output(changelog_changes)

    _save(filename=CHANGELOG_OUTPUT_FILE, content=changelog_output)


if __name__ == '__main__':
    extract_changelog(sys.argv[-1])
