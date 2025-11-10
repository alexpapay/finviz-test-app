import { useState, useRef, useDeferredValue } from "react";
import {
    Box,
    Paper,
    Typography,
    useMediaQuery,
    useTheme,
} from "@mui/material";
import { SearchInput } from "@/shared/ui/SearchInput";
import { SearchResults } from "@/features/search/ui/SearchResults";
import { TreeExplorer } from "@/features/treeExplorer/ui/TreeExplorer";
import type { TreeExplorerRef } from "@/features/treeExplorer/ui/TreeExplorer";
import { getNodePath } from "@/entities/imagenet/api/getNodePath";

export const ImageNetPage = () => {
    const [query, setQuery] = useState("");
    const deferredQuery = useDeferredValue(query);
    const treeRef = useRef<TreeExplorerRef>(null);
    const theme = useTheme();

    const isMobile = useMediaQuery(theme.breakpoints.down("md"));

    const handleSelectFromSearch = async (id: number) => {
        const path = await getNodePath(id);
        treeRef.current?.revealPath(path.map((x) => x.id));
    };

    return (
        <Box
            display="flex"
            flexDirection={isMobile ? "column" : "row"}
            height="100vh"
            bgcolor="background.default"
            color="text.primary"
        >
            <Box
                width={isMobile ? "100%" : 360}
                p={2}
                component={Paper}
                square
                sx={{
                    borderRight: isMobile ? 0 : 1,
                    borderBottom: isMobile ? 1 : 0,
                    borderColor: "divider",
                }}
            >
                <Typography variant="h6" gutterBottom>
                    Search
                </Typography>
                <SearchInput value={query} onChange={setQuery} placeholder="Search nodes..." />
                <Box mt={2} sx={{ maxHeight: isMobile ? "40vh" : "calc(100vh - 140px)", overflow: "auto" }}>
                    <SearchResults query={deferredQuery} onSelect={handleSelectFromSearch} />
                </Box>
            </Box>

            <Box flex={1} p={2} sx={{ overflow: "auto" }}>
                <Typography variant="h6" gutterBottom>
                    ImageNet Tree
                </Typography>
                <TreeExplorer ref={treeRef} />
            </Box>
        </Box>
    );
};
