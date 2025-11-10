import type { ReactNode } from "react";
import type { Theme } from "@mui/material/styles";

export function highlightText(
    text: string | undefined,
    query: string | undefined,
    theme: Theme
): ReactNode {
    if (!text) return "";
    if (!query) return text;

    const regex = new RegExp(`(${escapeRegExp(query)})`, "gi");
    const parts = text.split(regex);

    return parts.map((part, index) =>
        regex.test(part) ? (
            <span
                key={index}
                style={{
                    color: theme.palette.primary.main,
                    fontWeight: 600,
                }}
            >
                {part}
            </span>
        ) : (
            part
        )
    );
}

function escapeRegExp(string: string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}
