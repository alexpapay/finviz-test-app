import { useState, useRef, useDeferredValue } from "react";
import {
    Box,
    Paper,
    Typography,
    useMediaQuery,
    useTheme,
    Divider,
} from "@mui/material";
import { SearchInput } from "@/shared/ui/SearchInput";
import { SearchResults } from "@/features/search/ui/SearchResults";
import { TreeExplorer } from "@/features/treeExplorer/ui/TreeExplorer";
import type { TreeExplorerRef } from "@/features/treeExplorer/ui/TreeExplorer";
import { getNodePath } from "@/entities/imagenet/api/getNodePath";
import { ImportButton } from "@/features/import/ui/ImportButton";
import { useQueryClient } from "@tanstack/react-query";

export const ImageNetPage = () => {
    const [query, setQuery] = useState("");
    const deferredQuery = useDeferredValue(query);
    const treeRef = useRef<TreeExplorerRef>(null);
    const theme = useTheme();

    const isMobile = useMediaQuery(theme.breakpoints.down("md"));

    const queryClient = useQueryClient();
    const handleImportComplete = () => {
        queryClient.invalidateQueries({ queryKey: ["node-children"] });
    };

    const handleSelectFromSearch = async (id: number) => {
        const path = await getNodePath(id);
        treeRef.current?.revealPath(path.map((x) => x.id));
    };

    return (
        <Box
            display="flex"
            flexDirection="column"
            height="100vh"
            bgcolor="background.default"
            color="text.primary"
        >
            <Box
                display="flex"
                alignItems="center"
                justifyContent="space-between"
                p={2}
                sx={{
                    borderBottom: 1,
                    borderColor: "divider",
                    bgcolor: "background.paper",
                    boxShadow: "0 2px 6px rgba(0,0,0,0.05)",
                }}
            >
                <Typography variant="h6" fontWeight={600}>
                    ImageNet Tree
                </Typography>

                <ImportButton onImportComplete={handleImportComplete} />
            </Box>

            <Divider />

            <Box
                display="flex"
                flex={1}
                flexDirection={isMobile ? "column" : "row"}
                height="100%"
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
                    <Typography variant="subtitle1" fontWeight={500} gutterBottom>
                        Search
                    </Typography>

                    <SearchInput
                        value={query}
                        onChange={setQuery}
                        placeholder="Search nodes..."
                    />

                    <Box
                        mt={2}
                        sx={{
                            maxHeight: isMobile ? "40vh" : "calc(100vh - 200px)",
                            overflow: "auto",
                        }}
                    >
                        <SearchResults
                            query={deferredQuery}
                            onSelect={handleSelectFromSearch}
                        />
                    </Box>
                </Box>

                <Box flex={1} p={2} sx={{ overflow: "auto" }}>
                    <TreeExplorer ref={treeRef} />
                </Box>
            </Box>
        </Box>
    );
};
